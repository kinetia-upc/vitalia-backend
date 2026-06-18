using VitaliaBackend.Billing.Domain.Model.Commands;
using VitaliaBackend.Billing.Interfaces.Rest.Resources;

namespace VitaliaBackend.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts a <see cref="CreateBillingClaimResource" /> (public, HTTP facing) into a
///     <see cref="CreateBillingClaimCommand" /> (internal, used by the application layer).
/// </summary>
public static class CreateBillingClaimCommandFromResourceAssembler
{
    public static CreateBillingClaimCommand ToCommandFromResource(CreateBillingClaimResource resource)
    {
        return new CreateBillingClaimCommand(
            resource.ClaimCode,
            resource.InsuranceProvider,
            resource.PatientName,
            resource.ProviderName,
            resource.Value,
            resource.ClinicalCompliance,
            resource.CycleStatus);
    }
}
