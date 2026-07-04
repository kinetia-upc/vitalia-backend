using VitaliaBackend.Clinical.Application.Model;

namespace VitaliaBackend.Clinical.Application.QueryServices;

public interface IDiagnosisCatalogQueryService
{
    Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchByBranchAsync(
        string branchId,
        string? query,
        int limit,
        CancellationToken cancellationToken);
}
