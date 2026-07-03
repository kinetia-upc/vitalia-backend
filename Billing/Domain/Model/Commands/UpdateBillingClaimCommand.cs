namespace VitaliaBackend.Billing.Domain.Model.Commands;

/// <summary>
///     Carries the data needed to update an existing billing claim, identified by
///     <see cref="BillingClaimId" />.
/// </summary>
public record UpdateBillingClaimCommand(
    Guid BillingClaimId,
    string Code,
    Guid AppointmentId,
    string InsuranceProvider,
    string PatientName,
    string ProviderName,
    decimal Value,
    string ClinicalCompliance,
    string CycleStatus
);
