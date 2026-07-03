using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using VitaliaBackend.Tenant.Application.CommandServices;
using VitaliaBackend.Tenant.Application.QueryServices;
using VitaliaBackend.Tenant.Domain.Model;
using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Domain.Model.Queries;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;
using VitaliaBackend.Tenant.Interfaces.Rest.Transform;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

/// <summary>
///     REST entry point for healthcare centers, the root entity of the Tenant
///     bounded context.
/// </summary>
/// <remarks>
///     Route is "healthcareCenters" (camelCase) to match
///     VITE_VITALIA_HEALTHCARE_CENTER_ENDPOINT_PATH=/healthcareCenters in the frontend.
/// </remarks>
[ApiController]
[Route("api/v1/healthcareCenters")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Healthcare centers endpoints")]
public class HealthcareCentersController(
    IHealthcareCenterQueryService healthcareCenterQueryService,
    IHealthcareCenterCommandService healthcareCenterCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List healthcare centers", Description = "Returns every healthcare center registered in the platform.")]
    [SwaggerResponse(200, "The list of healthcare centers.", typeof(IEnumerable<HealthcareCenterResource>))]
    public async Task<IActionResult> GetHealthcareCenters(CancellationToken cancellationToken)
    {
        var query = new GetHealthcareCentersQuery();
        var healthcareCenters = await healthcareCenterQueryService.Handle(query, cancellationToken);
        var resources = healthcareCenters.Select(HealthcareCenterResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{healthcareCenterId}")]
    [SwaggerOperation(Summary = "Get a healthcare center by id", Description = "Returns a single healthcare center identified by its business id (e.g. 'hc-001').")]
    [SwaggerResponse(200, "The healthcare center was found.", typeof(HealthcareCenterResource))]
    [SwaggerResponse(404, "No healthcare center exists with the given id.")]
    public async Task<IActionResult> GetHealthcareCenterById(
        [FromRoute, SwaggerParameter("Business id of the healthcare center.")]
        string healthcareCenterId,
        CancellationToken cancellationToken)
    {
        var query = new GetHealthcareCenterByIdQuery(healthcareCenterId);
        var healthcareCenter = await healthcareCenterQueryService.Handle(query, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromNullable(
            this,
            healthcareCenter,
            TenantError.HealthcareCenterNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundCenter => Ok(HealthcareCenterResourceFromEntityAssembler.ToResourceFromEntity(foundCenter)));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a healthcare center", Description = "Creates a new healthcare center. The id must be unique.")]
    [SwaggerResponse(201, "The healthcare center was created.", typeof(HealthcareCenterResource))]
    [SwaggerResponse(400, "The data was invalid or the id is already used.")]
    public async Task<IActionResult> CreateHealthcareCenter(
        [FromBody, SwaggerParameter("Data for the new healthcare center.")]
        CreateHealthcareCenterResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateHealthcareCenterCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await healthcareCenterCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdCenter => CreatedAtAction(
                nameof(GetHealthcareCenterById),
                new { healthcareCenterId = createdCenter.Code },
                HealthcareCenterResourceFromEntityAssembler.ToResourceFromEntity(createdCenter)));
    }

    [HttpPut("{healthcareCenterId}")]
    [SwaggerOperation(Summary = "Update a healthcare center", Description = "Replaces every editable field of an existing healthcare center.")]
    [SwaggerResponse(200, "The healthcare center was updated.", typeof(HealthcareCenterResource))]
    [SwaggerResponse(400, "The data was invalid.")]
    [SwaggerResponse(404, "No healthcare center exists with the given id.")]
    public async Task<IActionResult> UpdateHealthcareCenter(
        [FromRoute, SwaggerParameter("Business id of the healthcare center to update.")]
        string healthcareCenterId,
        [FromBody, SwaggerParameter("New data for the healthcare center.")]
        UpdateHealthcareCenterResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateHealthcareCenterCommandFromResourceAssembler.ToCommandFromResource(healthcareCenterId, resource);
        var result = await healthcareCenterCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedCenter => Ok(HealthcareCenterResourceFromEntityAssembler.ToResourceFromEntity(updatedCenter)));
    }

    [HttpDelete("{healthcareCenterId}")]
    [SwaggerOperation(Summary = "Delete a healthcare center", Description = "Permanently removes a healthcare center by its business id.")]
    [SwaggerResponse(204, "The healthcare center was deleted.")]
    [SwaggerResponse(404, "No healthcare center exists with the given id.")]
    public async Task<IActionResult> DeleteHealthcareCenter(
        [FromRoute, SwaggerParameter("Business id of the healthcare center to delete.")]
        string healthcareCenterId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteHealthcareCenterCommand(healthcareCenterId);
        var result = await healthcareCenterCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            () => NoContent());
    }
}
