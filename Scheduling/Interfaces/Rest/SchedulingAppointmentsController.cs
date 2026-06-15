using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;
using VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

namespace VitaliaBackend.Scheduling.Interfaces.Rest;

[ApiController]
[Route("api/v1/scheduling/appointments")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Scheduling appointments endpoints")]
public class SchedulingAppointmentsController(
    IAppointmentQueryService appointmentQueryService,
    IAppointmentCommandService appointmentCommandService)
    : ControllerBase
{
    [HttpGet]
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
    public async Task<IActionResult> GetAppointmentById(
        [FromRoute] string appointmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentByIdQuery(appointmentId);
        var appointment = await appointmentQueryService.Handle(query, cancellationToken);

        if (appointment is null)
            return NotFound();

        return Ok(AppointmentResourceFromEntityAssembler.ToResourceFromEntity(appointment));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateAppointmentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var appointment = await appointmentCommandService.Handle(command, cancellationToken);

        if (appointment is null)
            return BadRequest();

        var appointmentResource = AppointmentResourceFromEntityAssembler.ToResourceFromEntity(appointment);

        return CreatedAtAction(
            nameof(GetAppointmentById),
            new { appointmentId = appointment.PublicId },
            appointmentResource);
    }

    [HttpPatch("{appointmentId}")]
    public async Task<IActionResult> UpdateAppointment(
        [FromRoute] string appointmentId,
        [FromBody] UpdateAppointmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateAppointmentCommandFromResourceAssembler.ToCommandFromResource(appointmentId, resource);
        var appointment = await appointmentCommandService.Handle(command, cancellationToken);

        if (appointment is null)
            return BadRequest();

        return Ok(AppointmentResourceFromEntityAssembler.ToResourceFromEntity(appointment));
    }

    [HttpDelete("{appointmentId}")]
    public async Task<IActionResult> DeleteAppointment(
        [FromRoute] string appointmentId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAppointmentCommand(appointmentId);
        var deleted = await appointmentCommandService.Handle(command, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}