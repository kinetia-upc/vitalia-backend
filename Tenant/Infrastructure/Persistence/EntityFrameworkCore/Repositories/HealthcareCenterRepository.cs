using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class HealthcareCenterRepository(AppDbContext context)
    : BaseRepository<HealthcareCenter>(context), IHealthcareCenterRepository
{
    public async Task<HealthcareCenter?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<HealthcareCenter>()
            .FirstOrDefaultAsync(healthcareCenter => healthcareCenter.Code == publicId, cancellationToken);
    }

    public async Task<bool> ExistsByPublicIdAsync(
        string publicId,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<HealthcareCenter>()
            .AnyAsync(healthcareCenter =>
                    healthcareCenter.Code == publicId
                    && (excludingPublicId == null || healthcareCenter.Code != excludingPublicId),
                cancellationToken);
    }
}
