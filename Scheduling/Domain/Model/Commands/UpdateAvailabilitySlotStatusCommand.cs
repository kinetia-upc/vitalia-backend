namespace VitaliaBackend.Scheduling.Domain.Model.Commands;

public record UpdateAvailabilitySlotStatusCommand(
    string AvailabilitySlotId,
    string Status
    );