using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class UpdateAvailabilitySlotCommandFromResourceAssembler
{
    public static UpdateAvailabilitySlotStatusCommand ToCommandFromResource(
        string availabilitySlotId,
        UpdateAvailabilitySlotResource resource)
    {
        return new UpdateAvailabilitySlotStatusCommand(
            availabilitySlotId,
            resource.Status);
    }
}