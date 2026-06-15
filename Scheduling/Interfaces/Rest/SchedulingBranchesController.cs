using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

namespace VitaliaBackend.Scheduling.Interfaces.Rest;

[ApiController]
[Route("api/v1/scheduling/branches")]
[Produces(MediaTypeNames.Application.Json)]
public class SchedulingBranchesController(ISchedulingBranchQueryService branchQueryService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllBranches(CancellationToken cancellationToken)
    {
        var branches = await branchQueryService.Handle(new GetAllSchedulingBranchesQuery(), cancellationToken);
        return Ok(branches.Select(SchedulingBranchResourceFromEntityAssembler.ToResourceFromEntity));
    }
}