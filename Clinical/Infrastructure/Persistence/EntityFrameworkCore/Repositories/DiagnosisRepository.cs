using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class DiagnosisRepository(AppDbContext context)
    : BaseRepository<Diagnosis>(context), IDiagnosisRepository
{
    public async Task<IEnumerable<Diagnosis>> FindAllByMedicalRecordIdAsync(
        string medicalRecordId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Diagnosis>()
            .Where(diagnosis => diagnosis.MedicalRecordId == medicalRecordId)
            .OrderByDescending(diagnosis => diagnosis.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
