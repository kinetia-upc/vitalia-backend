namespace VitaliaBackend.Billing.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON body the frontend sends when creating a billing claim.
/// </summary>
public record CreateBillingClaimResource(
    string ClaimCode,
    string InsuranceProvider,
    string PatientName,
    string ProviderName,
    decimal Value,
    string ClinicalCompliance,
    string CycleStatus
);
