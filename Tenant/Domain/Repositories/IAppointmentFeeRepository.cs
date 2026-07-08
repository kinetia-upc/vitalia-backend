using VitaliaBackend.Shared.Domain.Repositories;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Domain.Repositories;

public interface IAppointmentFeeRepository : IBaseRepository<AppointmentFee>
{
    Task<AppointmentFee?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);

    Task<AppointmentFee?> FindByBranchAndSpecialityAsync(
        string branchId,
        string? specialityId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByPublicIdAsync(
        string publicId,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Returns every appointment fee, or only the fees that belong to the given
    ///     branch when <paramref name="branchId" /> is provided.
    /// </summary>
    Task<IEnumerable<AppointmentFee>> SearchAsync(
        string? branchId = null,
        CancellationToken cancellationToken = default);
}
