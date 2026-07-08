using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PrescriptionRepository(AppDbContext context)
    : BaseRepository<Prescription>(context), IPrescriptionRepository
{
    public async Task<IEnumerable<Prescription>> FindAllByMedicalRecordIdAsync(
        Guid medicalRecordId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Prescription>()
            .Where(prescription => prescription.MedicalRecordId == medicalRecordId)
            .OrderByDescending(prescription => prescription.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Prescription>()
            .AnyAsync(prescription => prescription.Code == code, cancellationToken);
    }

    public async Task<int> GetMaxCodeNumberAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var codes = await Context.Set<Prescription>()
            .Where(prescription => prescription.Code.StartsWith(prefix))
            .Select(prescription => prescription.Code)
            .ToListAsync(cancellationToken);

        return codes
            .Select(code => code[prefix.Length..])
            .Where(suffix => int.TryParse(suffix, out _))
            .Select(int.Parse)
            .DefaultIfEmpty(0)
            .Max();
    }
}
