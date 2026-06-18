using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;

namespace VitaliaBackend.Clinical.Application.QueryServices;

public interface IPrescriptionDetailQueryService
{
    Task<PrescriptionDetail?> Handle(GetPrescriptionDetailByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<PrescriptionDetail>> Handle(
        GetPrescriptionDetailsByPrescriptionIdQuery query,
        CancellationToken cancellationToken);
}
