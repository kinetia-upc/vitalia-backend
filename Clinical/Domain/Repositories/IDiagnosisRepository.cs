using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IDiagnosisRepository : IBaseRepository<Diagnosis>
{
    Task<IEnumerable<Diagnosis>> FindAllByMedicalRecordIdAsync(
        Guid medicalRecordId,
        CancellationToken cancellationToken = default);
}
