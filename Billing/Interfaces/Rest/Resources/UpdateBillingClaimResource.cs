namespace VitaliaBackend.Billing.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON body the frontend sends when updating a billing claim.
///     This is also what the "Authorize" action in the frontend sends, with
///     CycleStatus set to "In Clearinghouse".
/// </summary>
public record UpdateBillingClaimResource(
    string ClaimCode,
    string InsuranceProvider,
    string PatientName,
    string ProviderName,
    decimal Value,
    string ClinicalCompliance,
    string CycleStatus
);
