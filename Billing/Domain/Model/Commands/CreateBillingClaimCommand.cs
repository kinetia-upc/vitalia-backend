namespace VitaliaBackend.Billing.Domain.Model.Commands;

/// <summary>
///     Carries the data needed to create a new billing claim. It is built by the
///     interface layer and consumed by the application layer.
/// </summary>
public record CreateBillingClaimCommand(
    string ClaimCode,
    string InsuranceProvider,
    string PatientName,
    string ProviderName,
    decimal Value,
    string ClinicalCompliance,
    string CycleStatus
);
