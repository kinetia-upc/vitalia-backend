namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record CreateAppointmentResource(
    string Id,
    string DoctorId,
    string PatientId,
    string BranchId,
    DateTime ScheduledAt,
    string Reason,
    string Status,
    string PaymentStatus
    );