using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.QueryServices;

public class SchedulingPatientQueryService(ISchedulingPatientRepository patientRepository) : ISchedulingPatientQueryService
{
    public async Task<IEnumerable<SchedulingPatient>> Handle(GetAllSchedulingPatientsQuery query, CancellationToken cancellationToken)
    {
        return await patientRepository.ListAsync(cancellationToken);
    }
}