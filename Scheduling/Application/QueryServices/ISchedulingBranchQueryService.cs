using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Model.Queries;

namespace VitaliaBackend.Scheduling.Application.QueryServices;

public interface ISchedulingBranchQueryService
{
    Task<IEnumerable<SchedulingBranch>> Handle(GetAllSchedulingBranchesQuery query, CancellationToken cancellationToken);
}