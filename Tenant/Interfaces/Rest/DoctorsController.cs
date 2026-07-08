using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/doctors")]
[Produces(MediaTypeNames.Application.Json)]
public class DoctorsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDoctors(CancellationToken cancellationToken)
    {
        return Ok(await context.Doctors.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetDoctorById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors.AsNoTracking().FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] Doctor resource, CancellationToken cancellationToken)
    {
        context.Doctors.Add(new Doctor(resource.UserId, resource.Code, resource.LicenseNumber, resource.CmpNumber));
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetDoctorById), new { userId = resource.UserId }, resource);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateDoctor([FromRoute] Guid userId, [FromBody] Doctor resource, CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors.FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (doctor is null) return NotFound();

        doctor.UpdateDetails(resource.LicenseNumber, resource.CmpNumber);

        await context.SaveChangesAsync(cancellationToken);
        return Ok(doctor);
    }
}
