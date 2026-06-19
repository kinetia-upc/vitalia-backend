using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.QueryServices;

public class MedicalRecordQueryService(IMedicalRecordRepository medicalRecordRepository)
    : IMedicalRecordQueryService
{
    public async Task<MedicalRecord?> Handle(
        GetMedicalRecordByCodeQuery query,
        CancellationToken cancellationToken)
    {
        return await medicalRecordRepository.FindByCodeAsync(query.Code, cancellationToken);
    }

    public async Task<MedicalRecord?> Handle(
        GetMedicalRecordByAppointmentIdQuery query,
        CancellationToken cancellationToken)
    {
        return await medicalRecordRepository.FindByAppointmentIdAsync(query.AppointmentId, cancellationToken);
    }

    public async Task<IEnumerable<MedicalRecord>> Handle(
        GetMedicalRecordsByPatientIdQuery query,
        CancellationToken cancellationToken)
    {
        return await medicalRecordRepository.FindAllByPatientIdAsync(query.PatientId, cancellationToken);
    }

    public async Task<IEnumerable<MedicalRecord>> Handle(
        GetAllMedicalRecordsQuery query,
        CancellationToken cancellationToken)
    {
        return await medicalRecordRepository.ListAsync(cancellationToken);
    }
}
