using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AvailabilitySlotRepository(AppDbContext context)
    : BaseRepository<AvailabilitySlot>(context), IAvailabilitySlotRepository
{
    public async Task<AvailabilitySlot?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AvailabilitySlot>()
            .FirstOrDefaultAsync(slot => slot.Code == publicId, cancellationToken);
    }

    public async Task<IEnumerable<AvailabilitySlot>> SearchAsync(
        string? doctorId,
        string? branchId,
        DateOnly? date,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<AvailabilitySlot>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(doctorId))
            query = query.Where(slot => slot.DoctorId.ToString() == doctorId);

        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(slot => slot.BranchId == branchId);

        if (date.HasValue)
            query = query.Where(slot => slot.Date == date.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<AvailabilitySlot?> FindByDoctorBranchDateAndStartTimeAsync(
        string doctorId,
        string branchId,
        DateOnly date,
        TimeOnly startTime,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AvailabilitySlot>()
            .FirstOrDefaultAsync(slot =>
                    slot.DoctorId.ToString() == doctorId &&
                    slot.BranchId == branchId &&
                    slot.Date == date &&
                    slot.StartTime == startTime,
                cancellationToken);
    }

    public async Task<bool> ExistsActiveSlotForDoctorAtAsync(
        string doctorId,
        DateOnly date,
        TimeOnly startTime,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AvailabilitySlot>()
            .AnyAsync(slot =>
                    slot.DoctorId.ToString() == doctorId &&
                    slot.Date == date &&
                    slot.StartTime == startTime &&
                    (excludingPublicId == null || slot.Code != excludingPublicId),
                cancellationToken);
    }
}
