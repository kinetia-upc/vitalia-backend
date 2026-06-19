using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/treatments")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Treatments endpoints")]
public class TreatmentsController(
    ITreatmentQueryService treatmentQueryService,
    ITreatmentCommandService treatmentCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all treatments",
        Description = "Retrieves a list of all treatments available in the system."
    )]
    public async Task<IActionResult> GetTreatments(CancellationToken cancellationToken)
    {
        var query = new GetAllTreatmentsQuery();
        var treatments = await treatmentQueryService.Handle(query, cancellationToken);
        var resources = treatments.Select(TreatmentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{treatmentId:int}")]
    [SwaggerOperation(
        Summary = "Get a treatment by id",
        Description = "Returns a single treatment using its numeric identifier."
    )]
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

    [HttpGet("medical-records/{medicalRecordId}")]
    [SwaggerOperation(
        Summary = "List treatments by medical record",
        Description = "Returns all treatments associated with the specified medical record identifier."
    )]
    public async Task<IActionResult> GetTreatmentsByMedicalRecordId(
        [FromRoute] string medicalRecordId,
        CancellationToken cancellationToken)
    {
        var query = new GetTreatmentsByMedicalRecordIdQuery(medicalRecordId);
        var treatments = await treatmentQueryService.Handle(query, cancellationToken);
        var resources = treatments.Select(TreatmentResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a treatment",
        Description = "Creates a new treatment entry for the specified medical record."
    )]
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
    [SwaggerOperation(
        Summary = "Update a treatment description",
        Description = "Updates the description of an existing treatment using its numeric identifier."
    )]
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

    [HttpDelete("{treatmentId:int}")]
    [SwaggerOperation(
        Summary = "Delete a treatment",
        Description = "Deletes an existing treatment using its numeric identifier."
    )]
    public async Task<IActionResult> DeleteTreatment(
        [FromRoute] int treatmentId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTreatmentCommand(treatmentId);
        var result = await treatmentCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}
