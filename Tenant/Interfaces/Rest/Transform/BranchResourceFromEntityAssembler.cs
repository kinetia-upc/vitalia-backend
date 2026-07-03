using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class BranchResourceFromEntityAssembler
{
    public static BranchResource ToResourceFromEntity(Branch entity)
    {
        return new BranchResource(
            entity.Id,
            entity.Code,
            entity.HealthcareCenterId,
            entity.Name,
            entity.Address);
    }
}
