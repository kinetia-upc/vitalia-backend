using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Domain.Repositories;

public interface IAvailabilitySlotRepository : IBaseRepository<AvailabilitySlot>
{
    Task<AvailabilitySlot?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);

    Task<IEnumerable<AvailabilitySlot>> SearchAsync(
        string? doctorId,
        string? branchId,
        DateOnly? date,
        CancellationToken cancellationToken = default);

    Task<AvailabilitySlot?> FindByDoctorBranchDateAndStartTimeAsync(
        string doctorId,
        string branchId,
        DateOnly date,
        TimeOnly startTime,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveSlotForDoctorAtAsync(
        string doctorId,
        DateOnly date,
        TimeOnly startTime,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default);
}