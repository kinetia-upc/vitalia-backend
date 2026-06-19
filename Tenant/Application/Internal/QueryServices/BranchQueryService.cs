using VitaliaBackend.Tenant.Application.QueryServices;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Queries;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Application.Internal.QueryServices;

public class BranchQueryService(IBranchRepository branchRepository) : IBranchQueryService
{
    public async Task<Branch?> Handle(GetBranchByIdQuery query, CancellationToken cancellationToken)
    {
        return await branchRepository.FindByPublicIdAsync(query.BranchId, cancellationToken);
    }

    public async Task<IEnumerable<Branch>> Handle(GetBranchesQuery query, CancellationToken cancellationToken)
    {
        return await branchRepository.SearchAsync(query.HealthcareCenterId, cancellationToken);
    }
}
