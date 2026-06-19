using VitaliaBackend.Shared.Domain.Repositories;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Domain.Repositories;

public interface IBranchRepository : IBaseRepository<Branch>
{
    Task<Branch?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByPublicIdAsync(
        string publicId,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns every branch, or only the branches that belong to the given
    ///     healthcare center when <paramref name="healthcareCenterId" /> is provided.
    /// </summary>
    Task<IEnumerable<Branch>> SearchAsync(
        string? healthcareCenterId = null,
        CancellationToken cancellationToken = default);
}
