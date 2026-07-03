namespace VitaliaBackend.Scheduling.Domain.Model.Commands;

public record CreateAvailabilitySlotCommand(
    string Code,
    string DoctorId,
    string BranchId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Status
    );
