using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SchedulingBranchRepository(AppDbContext context)
    : BaseRepository<SchedulingBranch>(context), ISchedulingBranchRepository
{
    public async Task<SchedulingBranch?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SchedulingBranch>()
            .FirstOrDefaultAsync(branch => branch.PublicId == publicId, cancellationToken);
    }
}