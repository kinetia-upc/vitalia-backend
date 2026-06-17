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
[Route("api/v1/medical-records")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical medical records endpoints")]
public class ClinicalMedicalRecordsController(
    IMedicalRecordQueryService medicalRecordQueryService,
    IMedicalRecordCommandService medicalRecordCommandService) : ControllerBase
{
    [HttpGet("{code}")]
    public async Task<IActionResult> GetMedicalRecordByCode(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordByCodeQuery(code);
        var medicalRecord = await medicalRecordQueryService.Handle(query, cancellationToken);

        if (medicalRecord is null)
            return NotFound(ClinicalErrors.MedicalRecordNotFoundError);

        return Ok(MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(medicalRecord));
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

        if (medicalRecord is null)
            return NotFound(ClinicalErrors.MedicalRecordNotFoundError);

        return Ok(MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(medicalRecord));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicalRecord(
        [FromBody] CreateMedicalRecordResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateMedicalRecordCommandFromResourceAssembler.ToCommandFromResource(resource);
        var medicalRecord = await medicalRecordCommandService.Handle(command, cancellationToken);

        if (medicalRecord is null)
            return BadRequest(ClinicalErrors.MedicalRecordCreationError);

        var medicalRecordResource = MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(medicalRecord);

        return CreatedAtAction(
            nameof(GetMedicalRecordByCode),
            new { code = medicalRecord.Code },
            medicalRecordResource);
    }
}
