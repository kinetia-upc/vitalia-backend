using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSpeciality([FromRoute] Guid id, [FromBody] UpdateSpecialityResource resource, CancellationToken cancellationToken)
    {
        var speciality = await context.Specialities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (speciality is null) return NotFound();
        speciality.UpdateDetails(resource.Description);
        await context.SaveChangesAsync(cancellationToken);
        return Ok(speciality);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSpeciality([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var speciality = await context.Specialities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (speciality is null) return NotFound();
        context.Specialities.Remove(speciality);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
