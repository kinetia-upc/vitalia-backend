using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class MedicalRecordRepository(AppDbContext context)
    : BaseRepository<MedicalRecord>(context), IMedicalRecordRepository
{
    public async Task<MedicalRecord?> FindByCodeAsync(
        string code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<MedicalRecord>()
            .FirstOrDefaultAsync(medicalRecord => medicalRecord.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<MedicalRecord>> FindAllByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MedicalRecord>()
            .Where(medicalRecord => medicalRecord.PatientId == patientId)
            .OrderByDescending(medicalRecord => medicalRecord.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MedicalRecord?> FindByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MedicalRecord>()
            .FirstOrDefaultAsync(medicalRecord => medicalRecord.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MedicalRecord>()
            .AnyAsync(medicalRecord => medicalRecord.Code == code, cancellationToken);
    }

    public async Task<bool> ExistsByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<MedicalRecord>()
            .AnyAsync(medicalRecord => medicalRecord.AppointmentId == appointmentId, cancellationToken);
    }
}
