using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class CreateHealthcareCenterCommandFromResourceAssembler
{
    public static CreateHealthcareCenterCommand ToCommandFromResource(CreateHealthcareCenterResource resource)
    {
        return new CreateHealthcareCenterCommand(
            resource.Code,
            resource.HealthcareCenterName,
            resource.AllianceStartDate,
            resource.AllianceFinishDate,
            resource.RucNumber,
            resource.ImageUrl ?? resource.ImageURL);
    }
}
