using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class UpdateBranchCommandFromResourceAssembler
{
    public static UpdateBranchCommand ToCommandFromResource(Guid branchId, UpdateBranchResource resource)
    {
        return new UpdateBranchCommand(
            branchId,
            resource.HealthcareCenterId,
            resource.BranchName,
            resource.Address);
    }
}
