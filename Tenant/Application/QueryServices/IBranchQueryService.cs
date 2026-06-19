using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Queries;

namespace VitaliaBackend.Tenant.Application.QueryServices;

public interface IBranchQueryService
{
    Task<Branch?> Handle(GetBranchByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<Branch>> Handle(GetBranchesQuery query, CancellationToken cancellationToken);
}
