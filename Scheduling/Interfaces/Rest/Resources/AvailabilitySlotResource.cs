namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record AvailabilitySlotResource(
    Guid Id,
    string Code,
    string DoctorId,
    string BranchId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Status
    );
