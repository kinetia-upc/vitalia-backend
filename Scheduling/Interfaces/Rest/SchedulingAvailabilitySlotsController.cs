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
[Route("api/v1/availabilitySlots")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Availability slots endpoints")]
public class AvailabilitySlotsController(
    IAvailabilitySlotQueryService availabilitySlotQueryService,
    IAvailabilitySlotCommandService availabilitySlotCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List availability slots",
        Description = "Returns all availability slots. Optional query parameters can be used to filter the results by doctor, branch, or date."
    )]
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
    [SwaggerOperation(
        Summary = "Get an availability slot by id",
        Description = "Returns a single availability slot using its public identifier."
    )]
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
    [SwaggerOperation(
        Summary = "Create an availability slot",
        Description = "Creates a new availability slot for a doctor at a specific branch, date, and time when no conflicting slot already exists."
    )]
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
    [SwaggerOperation(
        Summary = "Update an availability slot",
        Description = "Updates the status of an existing availability slot using its public identifier."
    )]
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
    [SwaggerOperation(
        Summary = "Delete an availability slot",
        Description = "Deletes an existing availability slot using its public identifier."
    )]
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
