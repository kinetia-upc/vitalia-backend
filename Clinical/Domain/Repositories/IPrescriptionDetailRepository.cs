using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IPrescriptionDetailRepository : IBaseRepository<PrescriptionDetail>
{
    Task<IEnumerable<PrescriptionDetail>> FindAllByPrescriptionIdAsync(
        int prescriptionId,
        CancellationToken cancellationToken = default);
}
