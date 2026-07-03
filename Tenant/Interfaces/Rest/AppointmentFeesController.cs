using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using VitaliaBackend.Tenant.Application.CommandServices;
using VitaliaBackend.Tenant.Application.QueryServices;
using VitaliaBackend.Tenant.Domain.Model;
using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Domain.Model.Queries;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;
using VitaliaBackend.Tenant.Interfaces.Rest.Transform;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

/// <summary>
///     REST entry point for appointment fees (the price charged for an appointment
///     at a given branch and speciality).
/// </summary>
/// <remarks>
///     Route is "appointmentFees" to match VITE_VITALIA_APPOINTMENT_FEE_ENDPOINT_PATH=/appointmentFees.
/// </remarks>
[ApiController]
[Route("api/v1/appointmentFees")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Appointment fees endpoints")]
public class AppointmentFeesController(
    IAppointmentFeeQueryService appointmentFeeQueryService,
    IAppointmentFeeCommandService appointmentFeeCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List appointment fees",
        Description = "Returns every appointment fee. When 'branchId' is provided, only fees belonging to that branch are returned.")]
    [SwaggerResponse(200, "The list of appointment fees.", typeof(IEnumerable<AppointmentFeeResource>))]
    public async Task<IActionResult> GetAppointmentFees(
        [FromQuery, SwaggerParameter("Optional id of the branch to filter by.")]
        string? branchId,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentFeesQuery(branchId);
        var appointmentFees = await appointmentFeeQueryService.Handle(query, cancellationToken);
        var resources = appointmentFees.Select(AppointmentFeeResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{appointmentFeeId}")]
    [SwaggerOperation(Summary = "Get an appointment fee by id", Description = "Returns a single appointment fee identified by its business id (e.g. 'fee-001').")]
    [SwaggerResponse(200, "The appointment fee was found.", typeof(AppointmentFeeResource))]
    [SwaggerResponse(404, "No appointment fee exists with the given id.")]
    public async Task<IActionResult> GetAppointmentFeeById(
        [FromRoute, SwaggerParameter("Business id of the appointment fee.")]
        string appointmentFeeId,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentFeeByIdQuery(appointmentFeeId);
        var appointmentFee = await appointmentFeeQueryService.Handle(query, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromNullable(
            this,
            appointmentFee,
            TenantError.AppointmentFeeNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundFee => Ok(AppointmentFeeResourceFromEntityAssembler.ToResourceFromEntity(foundFee)));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create an appointment fee", Description = "Creates a new appointment fee for a branch. The id must be unique and the price cannot be negative.")]
    [SwaggerResponse(201, "The appointment fee was created.", typeof(AppointmentFeeResource))]
    [SwaggerResponse(400, "The data was invalid or the id is already used.")]
    public async Task<IActionResult> CreateAppointmentFee(
        [FromBody, SwaggerParameter("Data for the new appointment fee.")]
        CreateAppointmentFeeResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateAppointmentFeeCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await appointmentFeeCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdFee => CreatedAtAction(
                nameof(GetAppointmentFeeById),
                new { appointmentFeeId = createdFee.Code },
                AppointmentFeeResourceFromEntityAssembler.ToResourceFromEntity(createdFee)));
    }

    [HttpPut("{appointmentFeeId}")]
    [SwaggerOperation(Summary = "Update an appointment fee", Description = "Replaces every editable field of an existing appointment fee.")]
    [SwaggerResponse(200, "The appointment fee was updated.", typeof(AppointmentFeeResource))]
    [SwaggerResponse(400, "The data was invalid.")]
    [SwaggerResponse(404, "No appointment fee exists with the given id.")]
    public async Task<IActionResult> UpdateAppointmentFee(
        [FromRoute, SwaggerParameter("Business id of the appointment fee to update.")]
        string appointmentFeeId,
        [FromBody, SwaggerParameter("New data for the appointment fee.")]
        UpdateAppointmentFeeResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateAppointmentFeeCommandFromResourceAssembler.ToCommandFromResource(appointmentFeeId, resource);
        var result = await appointmentFeeCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedFee => Ok(AppointmentFeeResourceFromEntityAssembler.ToResourceFromEntity(updatedFee)));
    }

    [HttpDelete("{appointmentFeeId}")]
    [SwaggerOperation(Summary = "Delete an appointment fee", Description = "Permanently removes an appointment fee by its business id.")]
    [SwaggerResponse(204, "The appointment fee was deleted.")]
    [SwaggerResponse(404, "No appointment fee exists with the given id.")]
    public async Task<IActionResult> DeleteAppointmentFee(
        [FromRoute, SwaggerParameter("Business id of the appointment fee to delete.")]
        string appointmentFeeId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAppointmentFeeCommand(appointmentFeeId);
        var result = await appointmentFeeCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            () => NoContent());
    }
}
