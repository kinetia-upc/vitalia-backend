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
                new { branchId = createdBranch.PublicId },
                BranchResourceFromEntityAssembler.ToResourceFromEntity(createdBranch)));
    }

    [HttpPut("{branchId}")]
    [SwaggerOperation(Summary = "Update a branch", Description = "Replaces every editable field of an existing branch.")]
    [SwaggerResponse(200, "The branch was updated.", typeof(BranchResource))]
    [SwaggerResponse(400, "The data was invalid.")]
    [SwaggerResponse(404, "No branch exists with the given id.")]
    public async Task<IActionResult> UpdateBranch(
        [FromRoute, SwaggerParameter("Business id of the branch to update.")]
        string branchId,
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

    [HttpDelete("{branchId}")]
    [SwaggerOperation(Summary = "Delete a branch", Description = "Permanently removes a branch by its business id.")]
    [SwaggerResponse(204, "The branch was deleted.")]
    [SwaggerResponse(404, "No branch exists with the given id.")]
    public async Task<IActionResult> DeleteBranch(
        [FromRoute, SwaggerParameter("Business id of the branch to delete.")]
        string branchId,
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
