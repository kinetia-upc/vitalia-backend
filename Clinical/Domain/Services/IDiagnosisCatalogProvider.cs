using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Domain.Services;

public interface IDiagnosisCatalogProvider
{
    Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken);

    Task<DiagnosisCatalogItem?> FindByCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken);
}
