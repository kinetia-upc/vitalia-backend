using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AppointmentFeeRepository(AppDbContext context)
    : BaseRepository<AppointmentFee>(context), IAppointmentFeeRepository
{
    public async Task<AppointmentFee?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AppointmentFee>()
            .FirstOrDefaultAsync(appointmentFee => appointmentFee.Code == publicId, cancellationToken);
    }

    public async Task<AppointmentFee?> FindByBranchAndSpecialityAsync(
        string branchId,
        string? specialityId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AppointmentFee>()
            .FirstOrDefaultAsync(appointmentFee =>
                    appointmentFee.BranchId == branchId
                    && appointmentFee.SpecialityId == specialityId,
                cancellationToken);
    }

    public async Task<bool> ExistsByPublicIdAsync(
        string publicId,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<AppointmentFee>()
            .AnyAsync(appointmentFee =>
                    appointmentFee.Code == publicId
                    && (excludingPublicId == null || appointmentFee.Code != excludingPublicId),
                cancellationToken);
    }

    public async Task<IEnumerable<AppointmentFee>> SearchAsync(
        string? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<AppointmentFee>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(appointmentFee => appointmentFee.BranchId == branchId);

        return await query.OrderBy(appointmentFee => appointmentFee.Price).ToListAsync(cancellationToken);
    }
}
