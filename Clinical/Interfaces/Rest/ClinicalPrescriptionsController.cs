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
[Route("api/v1/prescriptions")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical prescriptions endpoints")]
public class ClinicalPrescriptionsController(
    IPrescriptionQueryService prescriptionQueryService,
    IPrescriptionCommandService prescriptionCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet("{prescriptionId:int}")]
    [SwaggerOperation(
        Summary = "Get a prescription by id",
        Description = "Returns a single prescription using its numeric identifier."
    )]
    public async Task<IActionResult> GetPrescriptionById(
        [FromRoute] int prescriptionId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionByIdQuery(prescriptionId);
        var prescription = await prescriptionQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            prescription,
            ClinicalError.PrescriptionNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundPrescription => Ok(PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(foundPrescription)));
    }

    [HttpGet("medical-records/{medicalRecordId}")]
    [SwaggerOperation(
        Summary = "List prescriptions by medical record",
        Description = "Returns all prescriptions associated with the specified medical record identifier."
    )]
    public async Task<IActionResult> GetPrescriptionsByMedicalRecordId(
        [FromRoute] string medicalRecordId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionsByMedicalRecordIdQuery(medicalRecordId);
        var prescriptions = await prescriptionQueryService.Handle(query, cancellationToken);
        var resources = prescriptions.Select(PrescriptionResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a prescription",
        Description = "Creates a new prescription for the specified medical record."
    )]
    public async Task<IActionResult> CreatePrescription(
        [FromBody] CreatePrescriptionResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreatePrescriptionCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await prescriptionCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdPrescription => CreatedAtAction(
                nameof(GetPrescriptionById),
                new { prescriptionId = createdPrescription.Id },
                PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(createdPrescription)));
    }

    [HttpDelete("{prescriptionId:int}")]
    [SwaggerOperation(
        Summary = "Delete a prescription",
        Description = "Deletes an existing prescription using its numeric identifier."
    )]
    public async Task<IActionResult> DeletePrescription(
        [FromRoute] int prescriptionId,
        CancellationToken cancellationToken)
    {
        var command = new DeletePrescriptionCommand(prescriptionId);
        var result = await prescriptionCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}
