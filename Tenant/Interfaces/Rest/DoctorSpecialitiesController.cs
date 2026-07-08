using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/doctorSpecialities")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Doctor specialities endpoints")]
public class DoctorSpecialitiesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List doctor specialities", Description = "Returns every doctor-speciality assignment in the platform.")]
    [SwaggerResponse(200, "The list of doctor specialities was returned.", typeof(IEnumerable<DoctorSpeciality>))]
    public async Task<IActionResult> GetDoctorSpecialities(CancellationToken cancellationToken)
    {
        return Ok(await context.DoctorSpecialities.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Assign speciality to doctor", Description = "Creates a relationship between a doctor and a medical speciality.")]
    [SwaggerResponse(201, "The doctor speciality assignment was created.", typeof(DoctorSpeciality))]
    public async Task<IActionResult> CreateDoctorSpeciality(
        [FromBody, SwaggerParameter("Doctor and speciality ids to link.")]
        DoctorSpeciality resource,
        CancellationToken cancellationToken)
    {
        var doctorSpeciality = new DoctorSpeciality(resource.DoctorId, resource.SpecialityId);
        context.DoctorSpecialities.Add(doctorSpeciality);
        await context.SaveChangesAsync(cancellationToken);
        return Created(string.Empty, doctorSpeciality);
    }
}
