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
[Route("api/v1/diagnoses")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Diagnoses endpoints")]
public class DiagnosesController(
    IDiagnosisQueryService diagnosisQueryService,
    IDiagnosisCommandService diagnosisCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all diagnoses",
        Description = "Retrieves a list of all diagnoses available in the system."
    )]
    public async Task<IActionResult> GetDiagnoses(CancellationToken cancellationToken)
    {
        var query = new GetAllDiagnosesQuery();
        var diagnoses = await diagnosisQueryService.Handle(query, cancellationToken);
        var resources = diagnoses.Select(DiagnosisResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{diagnosisId:guid}")]
    [SwaggerOperation(
        Summary = "Get a diagnosis by id",
        Description = "Returns a single diagnosis using its numeric identifier."
    )]
    public async Task<IActionResult> GetDiagnosisById(
        [FromRoute] Guid diagnosisId,
        CancellationToken cancellationToken)
    {
        var query = new GetDiagnosisByIdQuery(diagnosisId);
        var diagnosis = await diagnosisQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            diagnosis,
            ClinicalError.DiagnosisNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundDiagnosis => Ok(DiagnosisResourceFromEntityAssembler.ToResourceFromEntity(foundDiagnosis)));
    }

    [HttpGet("medical-records/{medicalRecordId:guid}")]
    [SwaggerOperation(
        Summary = "List diagnoses by medical record",
        Description = "Returns all diagnoses associated with the specified medical record UUID."
    )]
    public async Task<IActionResult> GetDiagnosesByMedicalRecordId(
        [FromRoute] Guid medicalRecordId,
        CancellationToken cancellationToken)
    {
        var query = new GetDiagnosesByMedicalRecordIdQuery(medicalRecordId);
        var diagnoses = await diagnosisQueryService.Handle(query, cancellationToken);
        var resources = diagnoses.Select(DiagnosisResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a diagnosis",
        Description = "Creates a new diagnosis entry for the specified medical record."
    )]
    public async Task<IActionResult> CreateDiagnosis(
        [FromBody] CreateDiagnosisResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateDiagnosisCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await diagnosisCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdDiagnosis => CreatedAtAction(
                nameof(GetDiagnosisById),
                new { diagnosisId = createdDiagnosis.Id },
                DiagnosisResourceFromEntityAssembler.ToResourceFromEntity(createdDiagnosis)));
    }

    [HttpPatch("{diagnosisId:guid}")]
    [SwaggerOperation(
        Summary = "Update a diagnosis description",
        Description = "Updates the description of an existing diagnosis using its numeric identifier."
    )]
    public async Task<IActionResult> UpdateDiagnosisDescription(
        [FromRoute] Guid diagnosisId,
        [FromBody] UpdateDescriptionResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateDiagnosisDescriptionCommandFromResourceAssembler.ToCommandFromResource(
            diagnosisId,
            resource);
        var result = await diagnosisCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedDiagnosis => Ok(DiagnosisResourceFromEntityAssembler.ToResourceFromEntity(updatedDiagnosis)));
    }

    [HttpDelete("{diagnosisId:guid}")]
    [SwaggerOperation(
        Summary = "Delete a diagnosis",
        Description = "Deletes an existing diagnosis using its numeric identifier."
    )]
    public async Task<IActionResult> DeleteDiagnosis(
        [FromRoute] Guid diagnosisId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDiagnosisCommand(diagnosisId);
        var result = await diagnosisCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}
