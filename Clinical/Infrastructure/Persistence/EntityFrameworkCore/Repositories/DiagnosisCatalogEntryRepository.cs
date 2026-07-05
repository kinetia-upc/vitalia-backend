using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class DiagnosisCatalogEntryRepository(AppDbContext context)
    : BaseRepository<DiagnosisCatalogEntry>(context), IDiagnosisCatalogEntryRepository
{
    public async Task<DiagnosisCatalogEntry?> FindBySourceAndCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return await Context.Set<DiagnosisCatalogEntry>()
            .FirstOrDefaultAsync(entry =>
                    entry.Source == source &&
                    entry.Code == normalizedCode,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<DiagnosisCatalogEntry>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var searchText = DiagnosisCatalogSearchNormalizer.NormalizeForSearch(query ?? string.Empty);
        var catalogQuery = Context.Set<DiagnosisCatalogEntry>()
            .Where(entry => entry.Source == source);

        if (!string.IsNullOrWhiteSpace(searchText))
            catalogQuery = catalogQuery.Where(entry => entry.SearchText.Contains(searchText));

        return await catalogQuery
            .OrderBy(entry => entry.Code)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
