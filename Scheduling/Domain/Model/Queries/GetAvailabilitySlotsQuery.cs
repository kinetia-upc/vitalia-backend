namespace VitaliaBackend.Scheduling.Domain.Model.Queries;

public record GetAvailabilitySlotsQuery(
    string? DoctorId = null,
    string? BranchId = null,
    DateOnly? Date = null
);