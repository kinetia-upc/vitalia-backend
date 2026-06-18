using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class TreatmentRepository(AppDbContext context)
    : BaseRepository<Treatment>(context), ITreatmentRepository
{
    public async Task<IEnumerable<Treatment>> FindAllByMedicalRecordIdAsync(
        string medicalRecordId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Treatment>()
            .Where(treatment => treatment.MedicalRecordId == medicalRecordId)
            .OrderByDescending(treatment => treatment.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
