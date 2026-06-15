using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SchedulingPatientRepository(AppDbContext context)
    : BaseRepository<SchedulingPatient>(context), ISchedulingPatientRepository
{
    public async Task<SchedulingPatient?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SchedulingPatient>()
            .FirstOrDefaultAsync(patient => patient.PublicId == publicId, cancellationToken);
    }
}