using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PrescriptionDetailRepository(AppDbContext context)
    : BaseRepository<PrescriptionDetail>(context), IPrescriptionDetailRepository
{
    public async Task<IEnumerable<PrescriptionDetail>> FindAllByPrescriptionIdAsync(
        Guid prescriptionId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<PrescriptionDetail>()
            .Where(prescriptionDetail => prescriptionDetail.PrescriptionId == prescriptionId)
            .OrderByDescending(prescriptionDetail => prescriptionDetail.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PrescriptionDetail?> FindByPrescriptionAndMedicineAsync(
        Guid prescriptionId,
        Guid medicineId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<PrescriptionDetail>()
            .FirstOrDefaultAsync(prescriptionDetail =>
                    prescriptionDetail.PrescriptionId == prescriptionId
                    && prescriptionDetail.MedicineId == medicineId,
                cancellationToken);
    }
}
