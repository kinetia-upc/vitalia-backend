using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Clinical.Domain.Services;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Infrastructure.DiagnosisCatalog;

public class CompositeDiagnosisCatalogProvider(
    IEnumerable<IDiagnosisCatalogProvider> providers) : IDiagnosisCatalogProvider
{
    public async Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken)
    {
        foreach (var provider in Providers)
        {
            var items = await provider.SearchAsync(source, query, limit, cancellationToken);
            if (items.Count > 0)
                return items;
        }

        return [];
    }

    public async Task<DiagnosisCatalogItem?> FindByCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken)
    {
        foreach (var provider in Providers)
        {
            var item = await provider.FindByCodeAsync(source, code, cancellationToken);
            if (item is not null)
                return item;
        }

        return null;
    }

    private IEnumerable<IDiagnosisCatalogProvider> Providers =>
        providers.Where(provider => provider.GetType() != typeof(CompositeDiagnosisCatalogProvider));
}
