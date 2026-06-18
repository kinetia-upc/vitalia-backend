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
[Route("api/v1/medical-records")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical medical records endpoints")]
public class ClinicalMedicalRecordsController(
    IMedicalRecordQueryService medicalRecordQueryService,
    IMedicalRecordCommandService medicalRecordCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet("{code}")]
    public async Task<IActionResult> GetMedicalRecordByCode(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordByCodeQuery(code);
        var medicalRecord = await medicalRecordQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            medicalRecord,
            ClinicalError.MedicalRecordNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundMedicalRecord => Ok(MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(foundMedicalRecord)));
    }

    [HttpGet("patients/{patientId}")]
    public async Task<IActionResult> GetMedicalRecordsByPatientId(
        [FromRoute] string patientId,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordsByPatientIdQuery(patientId);
        var medicalRecords = await medicalRecordQueryService.Handle(query, cancellationToken);
        var resources = medicalRecords.Select(MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("appointments/{appointmentId}")]
    public async Task<IActionResult> GetMedicalRecordByAppointmentId(
        [FromRoute] string appointmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordByAppointmentIdQuery(appointmentId);
        var medicalRecord = await medicalRecordQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            medicalRecord,
            ClinicalError.MedicalRecordNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundMedicalRecord => Ok(MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(foundMedicalRecord)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicalRecord(
        [FromBody] CreateMedicalRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateMedicalRecordCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await medicalRecordCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdMedicalRecord => CreatedAtAction(
                nameof(GetMedicalRecordByCode),
                new { code = createdMedicalRecord.Code },
                MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(createdMedicalRecord)));
    }
}
