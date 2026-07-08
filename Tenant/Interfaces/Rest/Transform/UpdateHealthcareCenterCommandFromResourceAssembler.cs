using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class UpdateHealthcareCenterCommandFromResourceAssembler
{
    public static UpdateHealthcareCenterCommand ToCommandFromResource(
        string healthcareCenterId,
        UpdateHealthcareCenterResource resource)
    {
        return new UpdateHealthcareCenterCommand(
            healthcareCenterId,
            resource.HealthcareCenterName,
            resource.AllianceStartDate,
            resource.AllianceFinishDate,
            resource.RucNumber,
            resource.ImageUrl);
    }
}
