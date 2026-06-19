namespace VitaliaBackend.Billing.Domain.Model.Queries;

/// <summary>
///     Asks for the list of billing claims, optionally filtered by a free text search
///     that is matched against the claim code, patient name, provider name and
///     insurance provider.
/// </summary>
public record GetBillingClaimsQuery(string? Search = null);
