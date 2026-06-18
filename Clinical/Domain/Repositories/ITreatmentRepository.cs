using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface ITreatmentRepository : IBaseRepository<Treatment>
{
    Task<IEnumerable<Treatment>> FindAllByMedicalRecordIdAsync(
        string medicalRecordId,
        CancellationToken cancellationToken = default);
}
