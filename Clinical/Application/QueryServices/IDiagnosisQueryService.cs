using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;

namespace VitaliaBackend.Clinical.Application.QueryServices;

public interface IDiagnosisQueryService
{
    Task<Diagnosis?> Handle(GetDiagnosisByIdQuery query, CancellationToken cancellationToken);
}
