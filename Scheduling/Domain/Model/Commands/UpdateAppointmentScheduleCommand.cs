namespace VitaliaBackend.Scheduling.Domain.Model.Commands;

public record UpdateAppointmentScheduleCommand(
    string AppointmentId,
    string DoctorId,
    string PatientId,
    string BranchId,
    DateTime ScheduledAt,
    string? Reason,
    string? Status,
    string? PaymentStatus
);