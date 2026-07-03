using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class CreateBranchCommandFromResourceAssembler
{
    public static CreateBranchCommand ToCommandFromResource(CreateBranchResource resource)
    {
        return new CreateBranchCommand(
            resource.Code,
            resource.HealthcareCenterId,
            resource.BranchName,
            resource.Address);
    }
}
