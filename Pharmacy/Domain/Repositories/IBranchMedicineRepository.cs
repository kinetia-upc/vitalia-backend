using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Pharmacy.Domain.Repositories;

public interface IBranchMedicineRepository : IBaseRepository<BranchMedicine>
{
    Task<BranchMedicine?> FindByBranchAndMedicineAsync(
        string branchId,
        Guid medicineId,
        CancellationToken cancellationToken = default);
}
