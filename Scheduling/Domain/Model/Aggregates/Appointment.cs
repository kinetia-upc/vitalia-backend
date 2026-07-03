using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Scheduling.Domain.Model.Aggregates;

public class Appointment : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }
    public string BranchId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public string Reason { get; private set; }
    public EAppointmentStatus Status { get; private set; }
    public EPaymentStatus PaymentStatus { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    protected Appointment()
    {
        Code = string.Empty;
        BranchId = string.Empty;
        Reason = string.Empty;
    }

    public Appointment(
        Guid id,
        string code,
        Guid doctorId,
        Guid patientId,
        string branchId,
        DateTime scheduledAt,
        string reason,
        EAppointmentStatus status = EAppointmentStatus.Scheduled,
        EPaymentStatus paymentStatus = EPaymentStatus.Pending)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        DoctorId = doctorId;
        PatientId = patientId;
        BranchId = branchId;
        ScheduledAt = scheduledAt;
        Reason = reason;
        Status = status;
        PaymentStatus = paymentStatus;
    }

    public Appointment(
        string code,
        string doctorId,
        string patientId,
        string branchId,
        DateTime scheduledAt,
        string reason,
        EAppointmentStatus status = EAppointmentStatus.Scheduled,
        EPaymentStatus paymentStatus = EPaymentStatus.Pending)
        : this(Guid.NewGuid(), code, Guid.Parse(doctorId), Guid.Parse(patientId), branchId, scheduledAt, reason, status, paymentStatus)
    {
    }

    public bool IsActive =>
        Status is EAppointmentStatus.Scheduled
            or EAppointmentStatus.Confirmed
            or EAppointmentStatus.Arrived
            or EAppointmentStatus.InAttention;

    public void Reschedule(Guid doctorId, Guid patientId, string branchId, DateTime scheduledAt, string? reason = null)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        BranchId = branchId;
        ScheduledAt = scheduledAt;

        if (!string.IsNullOrWhiteSpace(reason))
            Reason = reason;

        Status = EAppointmentStatus.Scheduled;
    }

    public void Reschedule(string doctorId, string patientId, string branchId, DateTime scheduledAt, string? reason = null)
    {
        Reschedule(Guid.Parse(doctorId), Guid.Parse(patientId), branchId, scheduledAt, reason);
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
