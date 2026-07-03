namespace VitaliaBackend.Billing.Domain.Model.Queries;

/// <summary>
///     Asks for a single billing claim, identified by <see cref="BillingClaimId" />.
/// </summary>
public record GetBillingClaimByIdQuery(Guid BillingClaimId);
