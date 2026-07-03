using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/specialities")]
[Produces(MediaTypeNames.Application.Json)]
public class SpecialitiesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSpecialities(CancellationToken cancellationToken)
    {
        return Ok(await context.Specialities.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSpecialityById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var speciality = await context.Specialities.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return speciality is null ? NotFound() : Ok(speciality);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpeciality([FromBody] Speciality resource, CancellationToken cancellationToken)
    {
        var speciality = new Speciality(resource.Id, resource.Code, resource.Description);
        context.Specialities.Add(speciality);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetSpecialityById), new { id = speciality.Id }, speciality);
    }
}
