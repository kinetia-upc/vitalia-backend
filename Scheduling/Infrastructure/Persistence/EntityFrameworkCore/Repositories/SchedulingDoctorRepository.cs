using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SchedulingDoctorRepository(AppDbContext context)
    : BaseRepository<SchedulingDoctor>(context), ISchedulingDoctorRepository
{
    public async Task<SchedulingDoctor?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SchedulingDoctor>()
            .FirstOrDefaultAsync(doctor => doctor.PublicId == publicId, cancellationToken);
    }
}