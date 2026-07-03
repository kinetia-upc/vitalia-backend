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
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/medicalRecords")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Medical records endpoints")]
public class MedicalRecordsController(
    IMedicalRecordQueryService medicalRecordQueryService,
    IMedicalRecordCommandService medicalRecordCommandService,
    IAppointmentRepository appointmentRepository,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all medical records",
        Description = "Retrieves a list of all medical records available in the system."
    )]
    public async Task<IActionResult> GetMedicalRecords(CancellationToken cancellationToken)
    {
        var query = new GetAllMedicalRecordsQuery();
        var medicalRecords = await medicalRecordQueryService.Handle(query, cancellationToken);
        var resources = new List<MedicalRecordResource>();
        foreach (var medicalRecord in medicalRecords)
        {
            resources.Add(await ToResourceAsync(medicalRecord, cancellationToken));
        }
        return Ok(resources);
    }

    [HttpGet("{code}")]
    [SwaggerOperation(
        Summary = "Get a medical record by code",
        Description = "Returns a single medical record using its unique medical record code."
    )]
    public async Task<IActionResult> GetMedicalRecordByCode(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordByCodeQuery(code);
        var medicalRecord = await medicalRecordQueryService.Handle(query, cancellationToken);
        if (medicalRecord is null)
        {
            return ClinicalActionResultAssembler.ToActionResultFromNullable(
                this,
                medicalRecord,
                ClinicalError.MedicalRecordNotFound,
                errorLocalizer,
                problemDetailsFactory,
                _ => Ok());
        }

        return Ok(await ToResourceAsync(medicalRecord, cancellationToken));
    }

    [HttpGet("patients/{patientId:guid}")]
    [SwaggerOperation(
        Summary = "List medical records by patient",
        Description = "Returns all medical records associated with the specified patient UUID."
    )]
    public async Task<IActionResult> GetMedicalRecordsByPatientId(
        [FromRoute] Guid patientId,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordsByPatientIdQuery(patientId);
        var medicalRecords = await medicalRecordQueryService.Handle(query, cancellationToken);
        var resources = new List<MedicalRecordResource>();
        foreach (var medicalRecord in medicalRecords)
        {
            resources.Add(await ToResourceAsync(medicalRecord, cancellationToken));
        }

        return Ok(resources);
    }

    [HttpGet("appointments/{appointmentId:guid}")]
    [SwaggerOperation(
        Summary = "Get a medical record by appointment",
        Description = "Returns the medical record associated with the specified appointment UUID."
    )]
    public async Task<IActionResult> GetMedicalRecordByAppointmentId(
        [FromRoute] Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalRecordByAppointmentIdQuery(appointmentId);
        var medicalRecord = await medicalRecordQueryService.Handle(query, cancellationToken);
        if (medicalRecord is null)
        {
            return ClinicalActionResultAssembler.ToActionResultFromNullable(
                this,
                medicalRecord,
                ClinicalError.MedicalRecordNotFound,
                errorLocalizer,
                problemDetailsFactory,
                _ => Ok());
        }

        return Ok(await ToResourceAsync(medicalRecord, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a medical record",
        Description = "Creates a new medical record for a patient and optionally links it to an appointment."
    )]
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

    private async Task<MedicalRecordResource> ToResourceAsync(
        VitaliaBackend.Clinical.Domain.Model.Aggregates.MedicalRecord medicalRecord,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.FindByIdAsync(medicalRecord.AppointmentId, cancellationToken);
        return MedicalRecordResourceFromEntityAssembler.ToResourceFromEntity(medicalRecord, appointment?.Code);
    }
}
