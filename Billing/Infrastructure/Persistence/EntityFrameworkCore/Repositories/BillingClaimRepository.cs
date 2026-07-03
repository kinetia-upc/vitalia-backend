using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IBillingClaimRepository" />.
///     Inherits the generic Add/FindById/Update/Remove/List operations from
///     <see cref="BaseRepository{TEntity}" /> and adds the queries that are specific
///     to billing claims, translated into SQL by EF Core through LINQ.
/// </summary>
public class BillingClaimRepository(AppDbContext context)
    : BaseRepository<BillingClaim>(context), IBillingClaimRepository
{
    public async Task<bool> ExistsByClaimCodeAsync(
        string claimCode,
        Guid? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedClaimCode = claimCode.Trim().ToLower();

        return await Context.Set<BillingClaim>()
            .AnyAsync(billingClaim =>
                    billingClaim.Code.ToLower() == normalizedClaimCode
                    && (!excludingId.HasValue || billingClaim.Id != excludingId.Value),
                cancellationToken);
    }

    public async Task<IEnumerable<BillingClaim>> SearchAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<BillingClaim>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(billingClaim =>
                billingClaim.Code.ToLower().Contains(normalizedSearch)
                || billingClaim.PatientName.ToLower().Contains(normalizedSearch)
                || billingClaim.ProviderName.ToLower().Contains(normalizedSearch)
                || billingClaim.InsuranceProvider.ToLower().Contains(normalizedSearch));
        }

        return await query
            .OrderBy(billingClaim => billingClaim.PatientName)
            .ToListAsync(cancellationToken);
    }
}
