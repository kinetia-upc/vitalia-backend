using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Clinical.Domain.Services;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Infrastructure.DiagnosisCatalog;

public class LocalDiagnosisCatalogProvider(
    IDiagnosisCatalogEntryRepository diagnosisCatalogEntryRepository) : IDiagnosisCatalogProvider
{
    public async Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken)
    {
        if (!IsLocal(source))
            return [];

        var entries = await diagnosisCatalogEntryRepository.SearchAsync(source, query, limit, cancellationToken);
        return entries
            .Select(entry => new DiagnosisCatalogItem(entry.Source, entry.Code, entry.Description))
            .ToList();
    }

    public async Task<DiagnosisCatalogItem?> FindByCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken)
    {
        if (!IsLocal(source))
            return null;

        var entry = await diagnosisCatalogEntryRepository.FindBySourceAndCodeAsync(
            source,
            code,
            cancellationToken);

        return entry is null
            ? null
            : new DiagnosisCatalogItem(entry.Source, entry.Code, entry.Description);
    }

    private static bool IsLocal(DiagnosisCatalogSource source)
    {
        return source == DiagnosisCatalogSource.MINSA_CIE10;
    }
}
