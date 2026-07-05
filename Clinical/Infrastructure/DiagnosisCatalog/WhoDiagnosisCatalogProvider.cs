using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Clinical.Domain.Services;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Infrastructure.DiagnosisCatalog;

public class WhoDiagnosisCatalogProvider : IDiagnosisCatalogProvider
{
    public Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<DiagnosisCatalogItem>>([]);
    }

    public Task<DiagnosisCatalogItem?> FindByCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<DiagnosisCatalogItem?>(null);
    }
}
