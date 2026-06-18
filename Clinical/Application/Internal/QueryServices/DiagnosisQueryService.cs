using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.QueryServices;

public class DiagnosisQueryService(IDiagnosisRepository diagnosisRepository) : IDiagnosisQueryService
{
    public async Task<Diagnosis?> Handle(GetDiagnosisByIdQuery query, CancellationToken cancellationToken)
    {
        return await diagnosisRepository.FindByIdAsync(query.DiagnosisId, cancellationToken);
    }
}
