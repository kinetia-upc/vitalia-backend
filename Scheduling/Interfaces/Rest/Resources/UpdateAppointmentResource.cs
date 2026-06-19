namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record UpdateAppointmentResource(
    string DoctorId,
    string PatientId,
    string BranchId,
    DateTime ScheduledAt,
    string? Reason,
    string? Status,
    string? PaymentStatus
);