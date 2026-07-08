using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/doctors")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Doctors endpoints")]
public class DoctorsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List doctors", Description = "Returns every doctor profile registered in the platform.")]
    [SwaggerResponse(200, "The list of doctors was returned.", typeof(IEnumerable<Doctor>))]
    public async Task<IActionResult> GetDoctors(CancellationToken cancellationToken)
    {
        return Ok(await context.Doctors.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{userId:guid}")]
    [SwaggerOperation(Summary = "Get a doctor by user id", Description = "Returns a doctor profile linked to an IAM user UUID.")]
    [SwaggerResponse(200, "The doctor was found.", typeof(Doctor))]
    [SwaggerResponse(404, "No doctor exists for the given user id.")]
    public async Task<IActionResult> GetDoctorById(
        [FromRoute, SwaggerParameter("UUID of the user linked to the doctor.")]
        Guid userId,
        CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors.AsNoTracking().FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a doctor", Description = "Creates a doctor profile for an existing IAM user.")]
    [SwaggerResponse(201, "The doctor was created.", typeof(Doctor))]
    public async Task<IActionResult> CreateDoctor(
        [FromBody, SwaggerParameter("Data for the new doctor profile.")]
        Doctor resource,
        CancellationToken cancellationToken)
    {
        context.Doctors.Add(new Doctor(resource.UserId, resource.Code, resource.LicenseNumber, resource.CmpNumber));
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetDoctorById), new { userId = resource.UserId }, resource);
    }

    [HttpPut("{userId:guid}")]
    [SwaggerOperation(Summary = "Update a doctor", Description = "Updates professional license data for an existing doctor profile.")]
    [SwaggerResponse(200, "The doctor was updated.", typeof(Doctor))]
    [SwaggerResponse(404, "No doctor exists for the given user id.")]
    public async Task<IActionResult> UpdateDoctor(
        [FromRoute, SwaggerParameter("UUID of the user linked to the doctor.")]
        Guid userId,
        [FromBody, SwaggerParameter("New data for the doctor profile.")]
        Doctor resource,
        CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors.FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (doctor is null) return NotFound();

        doctor.UpdateDetails(resource.LicenseNumber, resource.CmpNumber);

        await context.SaveChangesAsync(cancellationToken);
        return Ok(doctor);
    }
}
