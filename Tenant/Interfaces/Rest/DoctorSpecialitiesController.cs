using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/doctorSpecialities")]
[Produces(MediaTypeNames.Application.Json)]
public class DoctorSpecialitiesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDoctorSpecialities(CancellationToken cancellationToken)
    {
        return Ok(await context.DoctorSpecialities.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctorSpeciality([FromBody] DoctorSpeciality resource, CancellationToken cancellationToken)
    {
        var doctorSpeciality = new DoctorSpeciality(resource.DoctorId, resource.SpecialityId);
        context.DoctorSpecialities.Add(doctorSpeciality);
        await context.SaveChangesAsync(cancellationToken);
        return Created(string.Empty, doctorSpeciality);
    }
}
