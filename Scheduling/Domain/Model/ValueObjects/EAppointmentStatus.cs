namespace VitaliaBackend.Scheduling.Domain.Model.ValueObjects;

public enum EAppointmentStatus
{
    Scheduled,
    Confirmed,
    Arrived,
    InAttention,
    Released,
    Cancelled
}