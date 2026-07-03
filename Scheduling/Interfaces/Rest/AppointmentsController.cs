using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;
using VitaliaBackend.Scheduling.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Scheduling.Interfaces.Rest;

[ApiController]
[Route("api/v1/appointments")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Appointments endpoints")]
public class AppointmentsController(
    IAppointmentQueryService appointmentQueryService,
    IAppointmentCommandService appointmentCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List appointments",
        Description = "Returns all appointments. Optional query parameters can be used to filter the results by doctor, patient, branch, or scheduled date."
    )]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] string? doctorId,
        [FromQuery] string? patientId,
        [FromQuery] string? branchId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentsQuery(doctorId, patientId, branchId, date);
        var appointments = await appointmentQueryService.Handle(query, cancellationToken);

        var resources = appointments.Select(AppointmentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{appointmentId}")]
    [SwaggerOperation(
        Summary = "Get an appointment by id",
        Description = "Returns a single appointment using its public identifier."
    )]
    public async Task<IActionResult> GetAppointmentById(
        [FromRoute] string appointmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentByIdQuery(appointmentId);
        var appointment = await appointmentQueryService.Handle(query, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromNullable(
            this,
            appointment,
            SchedulingError.AppointmentNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundAppointment => Ok(AppointmentResourceFromEntityAssembler.ToResourceFromEntity(foundAppointment)));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create an appointment",
        Description = "Creates a new appointment when the requested doctor, date, and time do not conflict with another active appointment."
    )]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateAppointmentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await appointmentCommandService.Handle(command, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdAppointment => CreatedAtAction(
                nameof(GetAppointmentById),
                new { appointmentId = createdAppointment.Code },
                AppointmentResourceFromEntityAssembler.ToResourceFromEntity(createdAppointment)));
    }

    [HttpPatch("{appointmentId}")]
    [SwaggerOperation(
        Summary = "Update an appointment",
        Description = "Updates the schedule or status of an existing appointment using its public identifier."
    )]
    public async Task<IActionResult> UpdateAppointment(
        [FromRoute] string appointmentId,
        [FromBody] UpdateAppointmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateAppointmentCommandFromResourceAssembler.ToCommandFromResource(appointmentId, resource);
        var result = await appointmentCommandService.Handle(command, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedAppointment => Ok(AppointmentResourceFromEntityAssembler.ToResourceFromEntity(updatedAppointment)));
    }

    [HttpDelete("{appointmentId}")]
    [SwaggerOperation(
        Summary = "Delete an appointment",
        Description = "Deletes an existing appointment using its public identifier."
    )]
    public async Task<IActionResult> DeleteAppointment(
        [FromRoute] string appointmentId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAppointmentCommand(appointmentId);
        var result = await appointmentCommandService.Handle(command, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}
