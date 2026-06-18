using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;

namespace VitaliaBackend.Clinical.Application.QueryServices;

public interface IPrescriptionQueryService
{
    Task<Prescription?> Handle(GetPrescriptionByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<Prescription>> Handle(GetPrescriptionsByMedicalRecordIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<Prescription>> Handle(GetAllPrescriptionsQuery query, CancellationToken cancellationToken);
}
