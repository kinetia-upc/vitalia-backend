using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Domain.Repositories;

public interface ISchedulingDoctorRepository  : IBaseRepository<SchedulingDoctor>
{
    Task<SchedulingDoctor?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);
}