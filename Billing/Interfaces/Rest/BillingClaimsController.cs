using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Billing.Application.CommandServices;
using VitaliaBackend.Billing.Application.QueryServices;
using VitaliaBackend.Billing.Domain.Model;
using VitaliaBackend.Billing.Domain.Model.Commands;
using VitaliaBackend.Billing.Domain.Model.Queries;
using VitaliaBackend.Billing.Interfaces.Rest.Resources;
using VitaliaBackend.Billing.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Billing.Interfaces.Rest;

/// <summary>
///     REST entry point for billing claims. Receives HTTP requests, turns them into
///     Commands/Queries, calls the application layer, and turns the result back into
///     HTTP responses.
/// </summary>
/// <remarks>
///     The route is "billingClaims" (camelCase, no hyphen) on purpose, not the usual
///     kebab-case convention, because it must match the path already hardcoded in the
///     frontend: VITE_VITALIA_BILLING_CLAIMS_ENDPOINT_PATH=/billingClaims.
/// </remarks>
[ApiController]
[Route("api/v1/billingClaims")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Billing claims endpoints")]
public class BillingClaimsController(
    IBillingClaimQueryService billingClaimQueryService,
    IBillingClaimCommandService billingClaimCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List billing claims",
        Description = "Returns every billing claim. When 'search' is provided, only claims whose " +
                       "claim code, patient name, provider name or insurance provider contain that " +
                       "text (case insensitive) are returned.")]
    [SwaggerResponse(200, "The list of billing claims that matched the search.", typeof(IEnumerable<BillingClaimResource>))]
    public async Task<IActionResult> GetBillingClaims(
        [FromQuery, SwaggerParameter("Free text filter, matched against claim code, patient name, provider name and insurance provider.")]
        string? search,
        CancellationToken cancellationToken)
    {
        var query = new GetBillingClaimsQuery(search);
        var billingClaims = await billingClaimQueryService.Handle(query, cancellationToken);
        var resources = billingClaims.Select(BillingClaimResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{billingClaimId:guid}")]
    [SwaggerOperation(
        Summary = "Get a billing claim by id",
        Description = "Returns a single billing claim identified by its UUID.")]
    [SwaggerResponse(200, "The billing claim was found.", typeof(BillingClaimResource))]
    [SwaggerResponse(404, "No billing claim exists with the given id.")]
    public async Task<IActionResult> GetBillingClaimById(
        [FromRoute, SwaggerParameter("UUID of the billing claim.")]
        Guid billingClaimId,
        CancellationToken cancellationToken)
    {
        var query = new GetBillingClaimByIdQuery(billingClaimId);
        var billingClaim = await billingClaimQueryService.Handle(query, cancellationToken);

        return BillingActionResultAssembler.ToActionResultFromNullable(
            this,
            billingClaim,
            BillingError.BillingClaimNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundClaim => Ok(BillingClaimResourceFromEntityAssembler.ToResourceFromEntity(foundClaim)));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a billing claim",
        Description = "Creates a new billing claim. The claim code must be unique, the value cannot " +
                       "be negative, and clinicalCompliance/cycleStatus must each be one of their " +
                       "allowed values (see BillingClaimStatuses).")]
    [SwaggerResponse(201, "The billing claim was created.", typeof(BillingClaimResource))]
    [SwaggerResponse(400, "The data was invalid or the claim code is already used.")]
    public async Task<IActionResult> CreateBillingClaim(
        [FromBody, SwaggerParameter("Data for the new billing claim.")]
        CreateBillingClaimResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateBillingClaimCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await billingClaimCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdClaim => CreatedAtAction(
                nameof(GetBillingClaimById),
                new { billingClaimId = createdClaim.Id },
                BillingClaimResourceFromEntityAssembler.ToResourceFromEntity(createdClaim)));
    }

    [HttpPut("{billingClaimId:guid}")]
    [SwaggerOperation(
        Summary = "Update a billing claim",
        Description = "Replaces every editable field of an existing billing claim. This is also the " +
                       "endpoint the frontend 'Authorize' action calls, sending cycleStatus = " +
                       "'In Clearinghouse'.")]
    [SwaggerResponse(200, "The billing claim was updated.", typeof(BillingClaimResource))]
    [SwaggerResponse(400, "The billing claim does not exist, the data was invalid, or the claim code is already used by another claim.")]
    public async Task<IActionResult> UpdateBillingClaim(
        [FromRoute, SwaggerParameter("UUID of the billing claim to update.")]
        Guid billingClaimId,
        [FromBody, SwaggerParameter("New data for the billing claim.")]
        UpdateBillingClaimResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateBillingClaimCommandFromResourceAssembler.ToCommandFromResource(billingClaimId, resource);
        var result = await billingClaimCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedClaim => Ok(BillingClaimResourceFromEntityAssembler.ToResourceFromEntity(updatedClaim)));
    }

    [HttpDelete("{billingClaimId:guid}")]
    [SwaggerOperation(
        Summary = "Delete a billing claim",
        Description = "Permanently removes a billing claim by its UUID.")]
    [SwaggerResponse(204, "The billing claim was deleted.")]
    [SwaggerResponse(404, "No billing claim exists with the given id.")]
    public async Task<IActionResult> DeleteBillingClaim(
        [FromRoute, SwaggerParameter("UUID of the billing claim to delete.")]
        Guid billingClaimId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBillingClaimCommand(billingClaimId);
        var result = await billingClaimCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            () => NoContent());
    }
}
