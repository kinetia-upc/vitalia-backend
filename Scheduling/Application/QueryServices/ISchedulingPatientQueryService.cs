using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Model.Queries;

namespace VitaliaBackend.Scheduling.Application.QueryServices;

public interface ISchedulingPatientQueryService
{
    Task<IEnumerable<SchedulingPatient>> Handle(GetAllSchedulingPatientsQuery query, CancellationToken cancellationToken);
}