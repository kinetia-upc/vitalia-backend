namespace VitaliaBackend.Scheduling.Domain.Model.Queries;

public record GetAppointmentsQuery(
    string? DoctorId = null,
    string? PatientId = null,
    string? BranchId = null,
    DateOnly? Date = null
);