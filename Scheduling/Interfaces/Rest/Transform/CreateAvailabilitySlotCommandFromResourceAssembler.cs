using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class CreateAvailabilitySlotCommandFromResourceAssembler
{
    public static CreateAvailabilitySlotCommand ToCommandFromResource(CreateAvailabilitySlotResource resource)
    {
        return new CreateAvailabilitySlotCommand(
            resource.Id,
            resource.DoctorId,
            resource.BranchId,
            resource.Date,
            resource.StartTime,
            resource.EndTime,
            resource.Status);
    }
}