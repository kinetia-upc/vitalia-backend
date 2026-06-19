namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record AppointmentResource(
    string Id, 
    string DoctorId, 
    string PatientId,
    string BranchId,
    DateTime ScheduledAt,
    string Reason,
    string Status,
    string PaymentStatus
    );