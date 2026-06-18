using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;

namespace VitaliaBackend.Clinical.Application.QueryServices;

public interface IMedicalRecordQueryService
{
    Task<MedicalRecord?> Handle(GetMedicalRecordByCodeQuery query, CancellationToken cancellationToken);
    Task<MedicalRecord?> Handle(GetMedicalRecordByAppointmentIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<MedicalRecord>> Handle(GetMedicalRecordsByPatientIdQuery query, CancellationToken cancellationToken);
}
