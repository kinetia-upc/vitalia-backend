using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Model.Queries;

namespace VitaliaBackend.Scheduling.Application.QueryServices;

public interface ISchedulingDoctorQueryService
{
    Task<IEnumerable<SchedulingDoctor>> Handle(GetAllSchedulingDoctorsQuery query, CancellationToken cancellationToken);
}