using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IPrescriptionRepository : IBaseRepository<Prescription>
{
    Task<IEnumerable<Prescription>> FindAllByMedicalRecordIdAsync(
        string medicalRecordId,
        CancellationToken cancellationToken = default);
}
