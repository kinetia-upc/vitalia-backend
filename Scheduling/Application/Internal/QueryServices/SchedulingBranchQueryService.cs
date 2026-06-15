using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.QueryServices;

public class SchedulingBranchQueryService(ISchedulingBranchRepository branchRepository) : ISchedulingBranchQueryService
{
    public async Task<IEnumerable<SchedulingBranch>> Handle(GetAllSchedulingBranchesQuery query, CancellationToken cancellationToken)
    {
        return await branchRepository.ListAsync(cancellationToken);
    }
}