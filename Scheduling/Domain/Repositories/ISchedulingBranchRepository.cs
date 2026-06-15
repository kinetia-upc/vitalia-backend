using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Domain.Repositories;

public interface ISchedulingBranchRepository : IBaseRepository<SchedulingBranch>
{
    Task<SchedulingBranch?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);
}