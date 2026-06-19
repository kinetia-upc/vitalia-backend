namespace VitaliaBackend.Billing.Domain.Model.Commands;

/// <summary>
///     Carries the data needed to update an existing billing claim, identified by
///     <see cref="BillingClaimId" />.
/// </summary>
public record UpdateBillingClaimCommand(
    int BillingClaimId,
    string ClaimCode,
    string InsuranceProvider,
    string PatientName,
    string ProviderName,
    decimal Value,
    string ClinicalCompliance,
    string CycleStatus
);
