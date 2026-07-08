using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IPrescriptionRepository : IBaseRepository<Prescription>
{
    Task<IEnumerable<Prescription>> FindAllByMedicalRecordIdAsync(
        Guid medicalRecordId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<int> GetMaxCodeNumberAsync(string prefix, CancellationToken cancellationToken = default);
}
