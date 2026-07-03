using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class AvailabilitySlotResourceFromEntityAssembler
{
    public static AvailabilitySlotResource ToResourceFromEntity(AvailabilitySlot entity)
    {
        return new AvailabilitySlotResource(
            entity.Id,
            entity.Code,
            entity.DoctorId.ToString(),
            entity.BranchId,
            entity.Date,
            entity.StartTime,
            entity.EndTime,
            entity.Status.ToString());
    }
}
