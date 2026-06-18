using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/treatments")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical treatments endpoints")]
public class ClinicalTreatmentsController(
    ITreatmentQueryService treatmentQueryService,
    ITreatmentCommandService treatmentCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet("{treatmentId:int}")]
    public async Task<IActionResult> GetTreatmentById(
        [FromRoute] int treatmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetTreatmentByIdQuery(treatmentId);
        var treatment = await treatmentQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            treatment,
            ClinicalError.TreatmentNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundTreatment => Ok(TreatmentResourceFromEntityAssembler.ToResourceFromEntity(foundTreatment)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTreatment(
        [FromBody] CreateTreatmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateTreatmentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await treatmentCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdTreatment => CreatedAtAction(
                nameof(GetTreatmentById),
                new { treatmentId = createdTreatment.Id },
                TreatmentResourceFromEntityAssembler.ToResourceFromEntity(createdTreatment)));
    }

    [HttpPatch("{treatmentId:int}")]
    public async Task<IActionResult> UpdateTreatmentDescription(
        [FromRoute] int treatmentId,
        [FromBody] UpdateDescriptionResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateTreatmentDescriptionCommandFromResourceAssembler.ToCommandFromResource(
            treatmentId,
            resource);
        var result = await treatmentCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedTreatment => Ok(TreatmentResourceFromEntityAssembler.ToResourceFromEntity(updatedTreatment)));
    }
}
