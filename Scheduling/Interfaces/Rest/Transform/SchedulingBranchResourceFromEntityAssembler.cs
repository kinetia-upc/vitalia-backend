using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class SchedulingBranchResourceFromEntityAssembler
{
    public static SchedulingBranchResource ToResourceFromEntity(SchedulingBranch entity)
    {
        return new SchedulingBranchResource(
            entity.PublicId,
            entity.Name,
            entity.Description);
    }
}