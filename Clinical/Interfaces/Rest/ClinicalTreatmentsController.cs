using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Errors;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/treatments")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical treatments endpoints")]
public class ClinicalTreatmentsController(
    ITreatmentQueryService treatmentQueryService,
    ITreatmentCommandService treatmentCommandService) : ControllerBase
{
    [HttpGet("{treatmentId:int}")]
    public async Task<IActionResult> GetTreatmentById(
        [FromRoute] int treatmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetTreatmentByIdQuery(treatmentId);
        var treatment = await treatmentQueryService.Handle(query, cancellationToken);

        if (treatment is null)
            return NotFound(ClinicalErrors.TreatmentNotFoundError);

        return Ok(TreatmentResourceFromEntityAssembler.ToResourceFromEntity(treatment));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTreatment(
        [FromBody] CreateTreatmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateTreatmentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var treatment = await treatmentCommandService.Handle(command, cancellationToken);

        if (treatment is null)
            return BadRequest(ClinicalErrors.TreatmentCreationError);

        var treatmentResource = TreatmentResourceFromEntityAssembler.ToResourceFromEntity(treatment);

        return CreatedAtAction(
            nameof(GetTreatmentById),
            new { treatmentId = treatment.Id },
            treatmentResource);
    }
}
