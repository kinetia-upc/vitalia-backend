namespace VitaliaBackend.Scheduling.Domain.Model.Commands;

public record CreateAvailabilitySlotCommand(
    string Id,
    string DoctorId,
    string BranchId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Status
    );