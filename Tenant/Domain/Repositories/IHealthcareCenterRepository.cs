using VitaliaBackend.Shared.Domain.Repositories;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Domain.Repositories;

public interface IHealthcareCenterRepository : IBaseRepository<HealthcareCenter>
{
    Task<HealthcareCenter?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByPublicIdAsync(
        string publicId,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default);
}
