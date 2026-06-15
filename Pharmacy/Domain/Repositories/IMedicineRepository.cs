using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Pharmacy.Domain.Repositories;

public interface IMedicineRepository : IBaseRepository<Medicine>
{
    Task<bool> ExistsByNameAndPresentationAsync(
        string name,
        int unitQuantity,
        string unitType,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Medicine>> SearchAsync(
        string? search = null,
        CancellationToken cancellationToken = default);
}
