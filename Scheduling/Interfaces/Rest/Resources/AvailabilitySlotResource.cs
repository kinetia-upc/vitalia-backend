namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record AvailabilitySlotResource(
    string Id,
    string DoctorId,
    string BranchId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Status
    );