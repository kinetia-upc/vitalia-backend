using VitaliaBackend.Billing.Domain.Model.Commands;
using VitaliaBackend.Billing.Interfaces.Rest.Resources;

namespace VitaliaBackend.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts an <see cref="UpdateBillingClaimResource" /> plus the id coming from the
///     route into an <see cref="UpdateBillingClaimCommand" />.
/// </summary>
public static class UpdateBillingClaimCommandFromResourceAssembler
{
    public static UpdateBillingClaimCommand ToCommandFromResource(
        int billingClaimId,
        UpdateBillingClaimResource resource)
    {
        return new UpdateBillingClaimCommand(
            billingClaimId,
            resource.ClaimCode,
            resource.InsuranceProvider,
            resource.PatientName,
            resource.ProviderName,
            resource.Value,
            resource.ClinicalCompliance,
            resource.CycleStatus);
    }
}
