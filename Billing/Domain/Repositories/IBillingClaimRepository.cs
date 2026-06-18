using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Billing.Domain.Repositories;

/// <summary>
///     Contract that any storage mechanism for <see cref="BillingClaim" /> must satisfy.
///     Inherits the generic Add/FindById/Update/Remove/List operations from
///     <see cref="IBaseRepository{TEntity}" /> and adds the operations that are
///     specific to billing claims.
/// </summary>
public interface IBillingClaimRepository : IBaseRepository<BillingClaim>
{
    /// <summary>
    ///     Checks whether a claim with the given claim code already exists, optionally
    ///     ignoring one specific id (used while updating, so a claim does not collide
    ///     with itself).
    /// </summary>
    Task<bool> ExistsByClaimCodeAsync(
        string claimCode,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns the claims that match the given free text search, or every claim
    ///     when no search text is provided.
    /// </summary>
    Task<IEnumerable<BillingClaim>> SearchAsync(
        string? search = null,
        CancellationToken cancellationToken = default);
}
