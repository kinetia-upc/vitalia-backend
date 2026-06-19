using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Interfaces.Rest.Resources;

namespace VitaliaBackend.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts a <see cref="BillingClaim" /> (internal aggregate) into a
///     <see cref="BillingClaimResource" /> (public, sent back to the frontend).
/// </summary>
public static class BillingClaimResourceFromEntityAssembler
{
    public static BillingClaimResource ToResourceFromEntity(BillingClaim entity)
    {
        return new BillingClaimResource(
            entity.Id,
            entity.ClaimCode,
            entity.InsuranceProvider,
            entity.PatientName,
            entity.ProviderName,
            entity.Value,
            entity.ClinicalCompliance,
            entity.CycleStatus);
    }
}
