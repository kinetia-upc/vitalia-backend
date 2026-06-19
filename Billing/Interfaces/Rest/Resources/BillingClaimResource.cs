using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Billing.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON the backend returns to the frontend for a billing claim.
///     Field names match exactly what vitalia-frontend/src/modules/billing expects
///     (id, claimCode, insuranceProvider, patientName, providerName, value,
///     clinicalCompliance, cycleStatus), because ASP.NET Core serializes C# property
///     names to camelCase JSON by default.
/// </summary>
public record BillingClaimResource(
    [property: SwaggerSchema(Description = "Internal numeric identifier assigned by the database.")]
    int Id,

    [property: SwaggerSchema(Description = "Unique business code for the claim, e.g. 'CLM-2026-0001'.")]
    string ClaimCode,

    [property: SwaggerSchema(Description = "Name of the insurance company being billed.")]
    string InsuranceProvider,

    [property: SwaggerSchema(Description = "Full name of the patient who received the service.")]
    string PatientName,

    [property: SwaggerSchema(Description = "Full name of the healthcare provider who delivered the service.")]
    string ProviderName,

    [property: SwaggerSchema(Description = "Amount being claimed. Must be zero or greater.")]
    decimal Value,

    [property: SwaggerSchema(Description = "Clinical documentation status. One of: verified, pending_sign, missing_icd10.")]
    string ClinicalCompliance,

    [property: SwaggerSchema(Description = "Revenue cycle status. One of: In Clearinghouse, Funds Released, Auth Required, Rejected.")]
    string CycleStatus
);
