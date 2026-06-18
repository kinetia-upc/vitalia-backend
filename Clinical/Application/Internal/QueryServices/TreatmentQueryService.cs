using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.QueryServices;

public class TreatmentQueryService(ITreatmentRepository treatmentRepository) : ITreatmentQueryService
{
    public async Task<Treatment?> Handle(GetTreatmentByIdQuery query, CancellationToken cancellationToken)
    {
        return await treatmentRepository.FindByIdAsync(query.TreatmentId, cancellationToken);
    }
}
