using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class BranchResourceFromEntityAssembler
{
    public static BranchResource ToResourceFromEntity(Branch entity)
    {
        return new BranchResource(
            entity.PublicId,
            entity.HealthcareCenterId,
            entity.AddressId,
            entity.Name,
            entity.Address);
    }
}
