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

    public async Task<IEnumerable<Diagnosis>> Handle(
        GetDiagnosesByMedicalRecordIdQuery query,
        CancellationToken cancellationToken)
    {
        return await diagnosisRepository.FindAllByMedicalRecordIdAsync(query.MedicalRecordId, cancellationToken);
    }

    public async Task<IEnumerable<Diagnosis>> Handle(
        GetAllDiagnosesQuery query,
        CancellationToken cancellationToken)
    {
        return await diagnosisRepository.ListAsync(cancellationToken);
    }
}
