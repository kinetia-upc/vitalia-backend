using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Domain.Repositories;

public interface ISchedulingPatientRepository : IBaseRepository<SchedulingPatient>
{
    Task<SchedulingPatient?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);
}