namespace VitaliaBackend.Billing.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON the backend returns to the frontend for a billing claim.
///     Field names match exactly what vitalia-frontend/src/modules/billing expects
///     (id, claimCode, insuranceProvider, patientName, providerName, value,
///     clinicalCompliance, cycleStatus), because ASP.NET Core serializes C# property
///     names to camelCase JSON by default.
/// </summary>
public record BillingClaimResource(
    int Id,
    string ClaimCode,
    string InsuranceProvider,
    string PatientName,
    string ProviderName,
    decimal Value,
    string ClinicalCompliance,
    string CycleStatus
);
