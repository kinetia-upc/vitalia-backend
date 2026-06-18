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
[Route("api/v1/scheduling/availabilitySlots")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Scheduling availability slots endpoints")]
public class SchedulingAvailabilitySlotsController(
    IAvailabilitySlotQueryService availabilitySlotQueryService,
    IAvailabilitySlotCommandService availabilitySlotCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
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

        return SchedulingActionResultAssembler.ToActionResultFromNullable(
            this,
            slot,
            SchedulingError.AvailabilitySlotNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundSlot => Ok(AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity(foundSlot)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAvailabilitySlot(
        [FromBody] CreateAvailabilitySlotResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateAvailabilitySlotCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await availabilitySlotCommandService.Handle(command, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdSlot => CreatedAtAction(
                nameof(GetAvailabilitySlotById),
                new { availabilitySlotId = createdSlot.PublicId },
                AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity(createdSlot)));
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

        var result = await availabilitySlotCommandService.Handle(command, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedSlot => Ok(AvailabilitySlotResourceFromEntityAssembler.ToResourceFromEntity(updatedSlot)));
    }

    [HttpDelete("{availabilitySlotId}")]
    public async Task<IActionResult> DeleteAvailabilitySlot(
        [FromRoute] string availabilitySlotId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAvailabilitySlotCommand(availabilitySlotId);
        var result = await availabilitySlotCommandService.Handle(command, cancellationToken);

        return SchedulingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}