using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;
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
///     REST entry point for branches (physical locations of a healthcare center).
/// </summary>
/// <remarks>
///     Route is "branches" to match VITE_VITALIA_BRANCH_ENDPOINT_PATH=/branches.
/// </remarks>
[ApiController]
[Route("api/v1/branches")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Branches endpoints")]
public class BranchesController(
    IBranchQueryService branchQueryService,
    IBranchCommandService branchCommandService,
    IDiagnosisCatalogQueryService diagnosisCatalogQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List branches",
        Description = "Returns every branch. When 'healthcareCenterId' is provided, only branches belonging to that healthcare center are returned.")]
    [SwaggerResponse(200, "The list of branches.", typeof(IEnumerable<BranchResource>))]
    public async Task<IActionResult> GetBranches(
        [FromQuery, SwaggerParameter("Optional id of the healthcare center to filter by.")]
        string? healthcareCenterId,
        CancellationToken cancellationToken)
    {
        var query = new GetBranchesQuery(healthcareCenterId);
        var branches = await branchQueryService.Handle(query, cancellationToken);
        var resources = branches.Select(BranchResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{branchId}/diagnosis-catalog")]
    [SwaggerOperation(
        Summary = "Autocomplete diagnosis catalog",
        Description = "Searches diagnosis catalog entries using the source configured for the branch.")]
    [SwaggerResponse(200, "The matching diagnosis catalog entries.", typeof(IEnumerable<DiagnosisCatalogEntryResource>))]
    public async Task<IActionResult> GetDiagnosisCatalog(
        [FromRoute, SwaggerParameter("Business id of the branch.")]
        string branchId,
        [FromQuery, SwaggerParameter("Text or code to search.")]
        string? query,
        [FromQuery, SwaggerParameter("Maximum number of entries to return.")]
        int limit,
        CancellationToken cancellationToken)
    {
        var branch = await branchQueryService.Handle(new GetBranchByIdQuery(branchId), cancellationToken);

        if (branch is null)
            return TenantActionResultAssembler.ToActionResultFromNullable(
                this,
                branch,
                TenantError.BranchNotFoundError,
                errorLocalizer,
                problemDetailsFactory,
                foundBranch => Ok(foundBranch));

        var items = await diagnosisCatalogQueryService.SearchByBranchAsync(branchId, query, limit, cancellationToken);
        var resources = items.Select(DiagnosisCatalogEntryResourceFromItemAssembler.ToResourceFromItem);

        return Ok(resources);
    }

    [HttpGet("{branchId}")]
    [SwaggerOperation(Summary = "Get a branch by id", Description = "Returns a single branch identified by its business id (e.g. 'branch-001').")]
    [SwaggerResponse(200, "The branch was found.", typeof(BranchResource))]
    [SwaggerResponse(404, "No branch exists with the given id.")]
    public async Task<IActionResult> GetBranchById(
        [FromRoute, SwaggerParameter("Business id of the branch.")]
        string branchId,
        CancellationToken cancellationToken)
    {
        var query = new GetBranchByIdQuery(branchId);
        var branch = await branchQueryService.Handle(query, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromNullable(
            this,
            branch,
            TenantError.BranchNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundBranch => Ok(BranchResourceFromEntityAssembler.ToResourceFromEntity(foundBranch)));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a branch", Description = "Creates a new branch under a healthcare center. The id must be unique.")]
    [SwaggerResponse(201, "The branch was created.", typeof(BranchResource))]
    [SwaggerResponse(400, "The data was invalid or the id is already used.")]
    public async Task<IActionResult> CreateBranch(
        [FromBody, SwaggerParameter("Data for the new branch.")]
        CreateBranchResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateBranchCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await branchCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdBranch => CreatedAtAction(
                nameof(GetBranchById),
                new { branchId = createdBranch.Code },
                BranchResourceFromEntityAssembler.ToResourceFromEntity(createdBranch)));
    }

    [HttpPut("{branchId:guid}")]
    [SwaggerOperation(Summary = "Update a branch", Description = "Replaces every editable field of an existing branch.")]
    [SwaggerResponse(200, "The branch was updated.", typeof(BranchResource))]
    [SwaggerResponse(400, "The data was invalid.")]
    [SwaggerResponse(404, "No branch exists with the given id.")]
    public async Task<IActionResult> UpdateBranch(
        [FromRoute, SwaggerParameter("UUID of the branch to update.")]
        Guid branchId,
        [FromBody, SwaggerParameter("New data for the branch.")]
        UpdateBranchResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateBranchCommandFromResourceAssembler.ToCommandFromResource(branchId, resource);
        var result = await branchCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedBranch => Ok(BranchResourceFromEntityAssembler.ToResourceFromEntity(updatedBranch)));
    }

    [HttpDelete("{branchId:guid}")]
    [SwaggerOperation(Summary = "Delete a branch", Description = "Permanently removes a branch by its UUID.")]
    [SwaggerResponse(204, "The branch was deleted.")]
    [SwaggerResponse(404, "No branch exists with the given id.")]
    public async Task<IActionResult> DeleteBranch(
        [FromRoute, SwaggerParameter("UUID of the branch to delete.")]
        Guid branchId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBranchCommand(branchId);
        var result = await branchCommandService.Handle(command, cancellationToken);

        return TenantActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            () => NoContent());
    }
}
