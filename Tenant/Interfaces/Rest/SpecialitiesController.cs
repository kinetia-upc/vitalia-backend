using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/specialities")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Specialities endpoints")]
public class SpecialitiesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List specialities", Description = "Returns every medical speciality available for doctors and fees.")]
    [SwaggerResponse(200, "The list of specialities was returned.", typeof(IEnumerable<Speciality>))]
    public async Task<IActionResult> GetSpecialities(CancellationToken cancellationToken)
    {
        return Ok(await context.Specialities.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get a speciality by id", Description = "Returns a single medical speciality identified by its UUID.")]
    [SwaggerResponse(200, "The speciality was found.", typeof(Speciality))]
    [SwaggerResponse(404, "No speciality exists with the given id.")]
    public async Task<IActionResult> GetSpecialityById(
        [FromRoute, SwaggerParameter("UUID of the medical speciality.")]
        Guid id,
        CancellationToken cancellationToken)
    {
        var speciality = await context.Specialities.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return speciality is null ? NotFound() : Ok(speciality);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a speciality", Description = "Creates a new medical speciality. The code must be unique.")]
    [SwaggerResponse(201, "The speciality was created.", typeof(Speciality))]
    public async Task<IActionResult> CreateSpeciality(
        [FromBody, SwaggerParameter("Data for the new medical speciality.")]
        Speciality resource,
        CancellationToken cancellationToken)
    {
        var speciality = new Speciality(resource.Id, resource.Code, resource.Description);
        context.Specialities.Add(speciality);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetSpecialityById), new { id = speciality.Id }, speciality);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update a speciality", Description = "Updates the description of an existing medical speciality.")]
    [SwaggerResponse(200, "The speciality was updated.", typeof(Speciality))]
    [SwaggerResponse(404, "No speciality exists with the given id.")]
    public async Task<IActionResult> UpdateSpeciality(
        [FromRoute, SwaggerParameter("UUID of the medical speciality to update.")]
        Guid id,
        [FromBody, SwaggerParameter("New description for the medical speciality.")]
        UpdateSpecialityResource resource,
        CancellationToken cancellationToken)
    {
        var speciality = await context.Specialities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (speciality is null) return NotFound();
        speciality.UpdateDetails(resource.Description);
        await context.SaveChangesAsync(cancellationToken);
        return Ok(speciality);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a speciality", Description = "Permanently removes a medical speciality by its UUID.")]
    [SwaggerResponse(204, "The speciality was deleted.")]
    [SwaggerResponse(404, "No speciality exists with the given id.")]
    public async Task<IActionResult> DeleteSpeciality(
        [FromRoute, SwaggerParameter("UUID of the medical speciality to delete.")]
        Guid id,
        CancellationToken cancellationToken)
    {
        var speciality = await context.Specialities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (speciality is null) return NotFound();
        context.Specialities.Remove(speciality);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
