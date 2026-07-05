using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Services;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.QueryServices;

public class DiagnosisCatalogQueryService(
    IBranchRepository branchRepository,
    IDiagnosisCatalogProvider diagnosisCatalogProvider) : IDiagnosisCatalogQueryService
{
    private const int DefaultLimit = 20;
    private const int MaxLimit = 100;

    public async Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchByBranchAsync(
        string branchId,
        string? query,
        int limit,
        CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FindByPublicIdAsync(branchId, cancellationToken);

        if (branch is null)
            return [];

        var safeLimit = limit <= 0 ? DefaultLimit : Math.Min(limit, MaxLimit);
        return await diagnosisCatalogProvider.SearchAsync(
            branch.DiagnosisCatalogSource,
            query,
            safeLimit,
            cancellationToken);
    }
}
