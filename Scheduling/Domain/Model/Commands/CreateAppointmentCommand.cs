namespace VitaliaBackend.Scheduling.Domain.Model.Commands;

public record CreateAppointmentCommand(
    string Code,
    string DoctorId,
    string PatientId,
    string BranchId,
    DateTime ScheduledAt,
    string Reason,
    string Status,
    string PaymentStatus
    );
