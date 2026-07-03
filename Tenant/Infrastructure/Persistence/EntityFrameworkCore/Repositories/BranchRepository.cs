using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class BranchRepository(AppDbContext context)
    : BaseRepository<Branch>(context), IBranchRepository
{
    public async Task<Branch?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Branch>()
            .FirstOrDefaultAsync(branch => branch.Code == publicId, cancellationToken);
    }

    public async Task<bool> ExistsByPublicIdAsync(
        string publicId,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Branch>()
            .AnyAsync(branch =>
                    branch.Code == publicId
                    && (excludingPublicId == null || branch.Code != excludingPublicId),
                cancellationToken);
    }

    public async Task<IEnumerable<Branch>> SearchAsync(
        string? healthcareCenterId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Branch>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(healthcareCenterId))
            query = query.Where(branch => branch.HealthcareCenterId == healthcareCenterId);

        return await query.OrderBy(branch => branch.Name).ToListAsync(cancellationToken);
    }
}
