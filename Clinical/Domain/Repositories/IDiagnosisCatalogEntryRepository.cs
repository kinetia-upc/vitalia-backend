using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IDiagnosisCatalogEntryRepository : IBaseRepository<DiagnosisCatalogEntry>
{
    Task<DiagnosisCatalogEntry?> FindBySourceAndCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DiagnosisCatalogEntry>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken = default);
}
