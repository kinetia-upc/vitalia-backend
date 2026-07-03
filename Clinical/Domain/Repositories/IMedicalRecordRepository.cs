using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IMedicalRecordRepository : IBaseRepository<MedicalRecord>
{
    Task<MedicalRecord?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MedicalRecord>> FindAllByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<MedicalRecord?> FindByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);
}
