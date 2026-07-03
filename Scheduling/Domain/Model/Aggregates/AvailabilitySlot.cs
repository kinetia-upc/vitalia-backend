using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Scheduling.Domain.Model.Aggregates;

public class AvailabilitySlot : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid DoctorId { get; private set; }
    public string BranchId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public EAvailabilitySlotStatus Status { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    protected AvailabilitySlot()
    {
        Code = string.Empty;
        BranchId = string.Empty;
    }

    public AvailabilitySlot(
        Guid id,
        string code,
        Guid doctorId,
        string branchId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        EAvailabilitySlotStatus status = EAvailabilitySlotStatus.Available)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        DoctorId = doctorId;
        BranchId = branchId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }

    public AvailabilitySlot(
        string code,
        string doctorId,
        string branchId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        EAvailabilitySlotStatus status = EAvailabilitySlotStatus.Available)
        : this(Guid.NewGuid(), code, Guid.Parse(doctorId), branchId, date, startTime, endTime, status)
    {
    }

    public bool IsAvailable => Status == EAvailabilitySlotStatus.Available;

    public bool Matches(Guid doctorId, string branchId, DateOnly date, TimeOnly startTime)
    {
        return DoctorId == doctorId
               && BranchId == branchId
               && Date == date
               && StartTime == startTime;
    }

    public bool Matches(string doctorId, string branchId, DateOnly date, TimeOnly startTime)
    {
        return Matches(Guid.Parse(doctorId), branchId, date, startTime);
    }

    public void Reschedule(Guid doctorId, string branchId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        DoctorId = doctorId;
        BranchId = branchId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
    }

    public void Reschedule(string doctorId, string branchId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        Reschedule(Guid.Parse(doctorId), branchId, date, startTime, endTime);
    }

    public void Book()
    {
        Status = EAvailabilitySlotStatus.Booked;
    }

    public void Release()
    {
        Status = EAvailabilitySlotStatus.Available;
    }

    public void ChangeStatus(EAvailabilitySlotStatus status)
    {
        Status = status;
    }
}
