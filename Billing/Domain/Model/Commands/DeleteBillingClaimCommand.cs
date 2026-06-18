namespace VitaliaBackend.Billing.Domain.Model.Commands;

/// <summary>
///     Carries the id of the billing claim that should be removed.
/// </summary>
public record DeleteBillingClaimCommand(int BillingClaimId);
