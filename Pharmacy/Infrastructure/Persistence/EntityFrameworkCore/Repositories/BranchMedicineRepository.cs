using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Pharmacy.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class BranchMedicineRepository(AppDbContext context)
    : BaseRepository<BranchMedicine>(context), IBranchMedicineRepository
{
    public async Task<BranchMedicine?> FindByBranchAndMedicineAsync(
        string branchId,
        Guid medicineId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<BranchMedicine>()
            .FirstOrDefaultAsync(
                bm => bm.BranchId == branchId && bm.MedicineId == medicineId,
                cancellationToken);
    }
}
