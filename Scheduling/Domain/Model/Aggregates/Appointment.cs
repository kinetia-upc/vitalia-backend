using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Scheduling.Domain.Model.Aggregates;

public class Appointment : IAuditableEntity
{
    public int Id { get; private set; }
    public string PublicId { get; private set; }
    public string DoctorId { get; private set; }
    public string PatientId { get; private set; }
    public string BranchId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public string Reason { get; private set; }
    public EAppointmentStatus Status { get; private set; }
    public EPaymentStatus PaymentStatus { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    protected Appointment()
    {
        PublicId = string.Empty;
        DoctorId = string.Empty;
        PatientId = string.Empty;
        BranchId = string.Empty;
        Reason = string.Empty;
    }

    public Appointment(
        string publicId,
        string doctorId,
        string patientId,
        string branchId,
        DateTime scheduledAt,
        string reason,
        EAppointmentStatus status = EAppointmentStatus.Scheduled,
        EPaymentStatus paymentStatus = EPaymentStatus.Pending)
    {
        PublicId = publicId;
        DoctorId = doctorId;
        PatientId = patientId;
        BranchId = branchId;
        ScheduledAt = scheduledAt;
        Reason = reason;
        Status = status;
        PaymentStatus = paymentStatus;
    }

    public bool IsActive =>
        Status is EAppointmentStatus.Scheduled
            or EAppointmentStatus.Confirmed
            or EAppointmentStatus.Arrived
            or EAppointmentStatus.InAttention;

    public void Reschedule(string doctorId, string patientId, string branchId, DateTime scheduledAt, string? reason = null)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        BranchId = branchId;
        ScheduledAt = scheduledAt;

        if (!string.IsNullOrWhiteSpace(reason))
            Reason = reason;

        Status = EAppointmentStatus.Scheduled;
    }

    public void ChangeStatus(EAppointmentStatus status)
    {
        Status = status;
    }

    public void Confirm()
    {
        Status = EAppointmentStatus.Confirmed;
    }

    public void MarkArrived()
    {
        Status = EAppointmentStatus.Arrived;
    }

    public void StartAttention()
    {
        if (Status is EAppointmentStatus.Scheduled or EAppointmentStatus.Confirmed or EAppointmentStatus.Arrived)
            Status = EAppointmentStatus.InAttention;
    }

    public void Release()
    {
        if (Status is EAppointmentStatus.InAttention or EAppointmentStatus.Arrived)
            Status = EAppointmentStatus.Released;
    }

    public void Cancel()
    {
        Status = EAppointmentStatus.Cancelled;
    }

    public void MarkAsPaid()
    {
        PaymentStatus = EPaymentStatus.Paid;
    }

    public void ChangePaymentStatus(EPaymentStatus paymentStatus)
    {
        PaymentStatus = paymentStatus;
    }
}