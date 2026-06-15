using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.QueryServices;

public class SchedulingDoctorQueryService(ISchedulingDoctorRepository doctorRepository) : ISchedulingDoctorQueryService
{
    public async Task<IEnumerable<SchedulingDoctor>> Handle(GetAllSchedulingDoctorsQuery query, CancellationToken cancellationToken)
    {
        return await doctorRepository.ListAsync(cancellationToken);
    }
}