using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;

namespace VitaliaBackend.Clinical.Application.QueryServices;

public interface ITreatmentQueryService
{
    Task<Treatment?> Handle(GetTreatmentByIdQuery query, CancellationToken cancellationToken);
}
