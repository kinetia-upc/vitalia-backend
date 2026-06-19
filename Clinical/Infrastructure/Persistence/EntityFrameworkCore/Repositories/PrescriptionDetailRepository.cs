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
        int prescriptionId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<PrescriptionDetail>()
            .Where(prescriptionDetail => prescriptionDetail.PrescriptionId == prescriptionId)
            .OrderByDescending(prescriptionDetail => prescriptionDetail.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
