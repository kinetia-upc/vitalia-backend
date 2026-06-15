using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Scheduling.Domain.Model.Aggregates;

public class AvailabilitySlot : IAuditableEntity
{
    public int Id { get; private set; }
    public string PublicId { get; private set; }
    public string DoctorId { get; private set; }
    public string BranchId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public EAvailabilitySlotStatus Status { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    protected AvailabilitySlot()
    {
        PublicId = string.Empty;
        DoctorId = string.Empty;
        BranchId = string.Empty;
    }

    public AvailabilitySlot(
        string publicId,
        string doctorId,
        string branchId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        EAvailabilitySlotStatus status = EAvailabilitySlotStatus.Available)
    {
        PublicId = publicId;
        DoctorId = doctorId;
        BranchId = branchId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }

    public bool IsAvailable => Status == EAvailabilitySlotStatus.Available;

    public bool Matches(string doctorId, string branchId, DateOnly date, TimeOnly startTime)
    {
        return DoctorId == doctorId
               && BranchId == branchId
               && Date == date
               && StartTime == startTime;
    }

    public void Reschedule(string doctorId, string branchId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        DoctorId = doctorId;
        BranchId = branchId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
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