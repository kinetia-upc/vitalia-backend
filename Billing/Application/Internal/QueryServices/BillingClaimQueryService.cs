using VitaliaBackend.Billing.Application.QueryServices;
using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Model.Queries;
using VitaliaBackend.Billing.Domain.Repositories;

namespace VitaliaBackend.Billing.Application.Internal.QueryServices;

/// <summary>
///     Implements every read use case for billing claims by delegating directly to the
///     repository. No validation is needed here because reading data cannot break any
///     business rule.
/// </summary>
public class BillingClaimQueryService(IBillingClaimRepository billingClaimRepository) : IBillingClaimQueryService
{
    public async Task<BillingClaim?> Handle(GetBillingClaimByIdQuery query, CancellationToken cancellationToken)
    {
        return await billingClaimRepository.FindByIdAsync(query.BillingClaimId, cancellationToken);
    }

    public async Task<IEnumerable<BillingClaim>> Handle(GetBillingClaimsQuery query, CancellationToken cancellationToken)
    {
        return await billingClaimRepository.SearchAsync(query.Search, cancellationToken);
    }
}
