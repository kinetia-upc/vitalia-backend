using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Billing.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root that represents a single insurance billing claim.
/// </summary>
/// <remarks>
///     All properties use a private setter on purpose: the only supported ways to change
///     the state of a claim are the constructor (when it is first created) and
///     <see cref="UpdateDetails" /> (when it is edited later). This keeps every business
///     rule about what a valid claim looks like inside this single class.
/// </remarks>
public class BillingClaim : IAuditableEntity
{
    public int Id { get; private set; }

    /// <summary>
    ///     Human readable, unique business code for the claim (for example "CLM-2026-0001").
    /// </summary>
    public string ClaimCode { get; private set; }

    public string InsuranceProvider { get; private set; }
    public string PatientName { get; private set; }
    public string ProviderName { get; private set; }
    public decimal Value { get; private set; }

    /// <summary>
    ///     Clinical documentation status. Must be one of the values listed in
    ///     <see cref="ValueObjects.BillingClaimStatuses.AllowedClinicalCompliances" />.
    /// </summary>
    public string ClinicalCompliance { get; private set; }

    /// <summary>
    ///     Revenue cycle status. Must be one of the values listed in
    ///     <see cref="ValueObjects.BillingClaimStatuses.AllowedCycleStatuses" />.
    /// </summary>
    public string CycleStatus { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Empty constructor required by Entity Framework Core to materialize entities
    ///     coming from the database. It should never be called directly from application code.
    /// </summary>
    protected BillingClaim()
    {
        ClaimCode = string.Empty;
        InsuranceProvider = string.Empty;
        PatientName = string.Empty;
        ProviderName = string.Empty;
        ClinicalCompliance = string.Empty;
        CycleStatus = string.Empty;
    }

    /// <summary>
    ///     Creates a brand new billing claim.
    /// </summary>
    public BillingClaim(
        string claimCode,
        string insuranceProvider,
        string patientName,
        string providerName,
        decimal value,
        string clinicalCompliance,
        string cycleStatus)
    {
        ClaimCode = claimCode.Trim();
        InsuranceProvider = insuranceProvider.Trim();
        PatientName = patientName.Trim();
        ProviderName = providerName.Trim();
        Value = value;
        ClinicalCompliance = clinicalCompliance.Trim();
        CycleStatus = cycleStatus.Trim();
    }

    /// <summary>
    ///     Replaces every editable field of the claim with new values.
    /// </summary>
    public void UpdateDetails(
        string claimCode,
        string insuranceProvider,
        string patientName,
        string providerName,
        decimal value,
        string clinicalCompliance,
        string cycleStatus)
    {
        ClaimCode = claimCode.Trim();
        InsuranceProvider = insuranceProvider.Trim();
        PatientName = patientName.Trim();
        ProviderName = providerName.Trim();
        Value = value;
        ClinicalCompliance = clinicalCompliance.Trim();
        CycleStatus = cycleStatus.Trim();
    }
}
