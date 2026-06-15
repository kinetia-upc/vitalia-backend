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
[Route("api/v1/scheduling/availability-slots")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Scheduling availability slots endpoints")]
public class SchedulingAvailabilitySlotsController(
    IAvailabilitySlotQueryService availabilitySlotQueryService,
    IAvailabilitySlotCommandService availabilitySlotCommandService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAvailabilitySlots(
        [FromQuery] string? doctorId,
        [FromQuery] string? branchId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        var query = new GetAvailabilitySlotsQuery(doctorId, branchId, date);
        var slots = await availabilitySlotQueryService.Handle(query, cancellationToken);

        var resources = slots.Select(AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{availabilitySlotId}")]
    public async Task<IActionResult> GetAvailabilitySlotById(
        [FromRoute] string availabilitySlotId,
        CancellationToken cancellationToken)
    {
        var query = new GetAvailabilitySlotByIdQuery(availabilitySlotId);
        var slot = await availabilitySlotQueryService.Handle(query, cancellationToken);

        if (slot is null)
            return NotFound();

        return Ok(AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity(slot));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAvailabilitySlot(
        [FromBody] CreateAvailabilitySlotResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateAvailabilitySlotCommandFromResourceAssembler.ToCommandFromResource(resource);
        var slot = await availabilitySlotCommandService.Handle(command, cancellationToken);

        if (slot is null)
            return BadRequest();

        var slotResource = AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity(slot);

        return CreatedAtAction(
            nameof(GetAvailabilitySlotById),
            new { availabilitySlotId = slot.PublicId },
            slotResource);
    }

    [HttpPatch("{availabilitySlotId}")]
    public async Task<IActionResult> UpdateAvailabilitySlotStatus(
        [FromRoute] string availabilitySlotId,
        [FromBody] UpdateAvailabilitySlotResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateAvailabilitySlotCommandFromResourceAssembler.ToCommandFromResource(
            availabilitySlotId,
            resource);

        var slot = await availabilitySlotCommandService.Handle(command, cancellationToken);

        if (slot is null)
            return BadRequest();

        return Ok(AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity(slot));
    }

    [HttpDelete("{availabilitySlotId}")]
    public async Task<IActionResult> DeleteAvailabilitySlot(
        [FromRoute] string availabilitySlotId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAvailabilitySlotCommand(availabilitySlotId);
        var deleted = await availabilitySlotCommandService.Handle(command, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}