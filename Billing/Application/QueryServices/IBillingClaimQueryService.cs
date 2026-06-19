using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Model.Queries;

namespace VitaliaBackend.Billing.Application.QueryServices;

/// <summary>
///     Declares every read operation that the Billing bounded context can answer
///     about billing claims.
/// </summary>
public interface IBillingClaimQueryService
{
    Task<BillingClaim?> Handle(GetBillingClaimByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<BillingClaim>> Handle(GetBillingClaimsQuery query, CancellationToken cancellationToken);
}
