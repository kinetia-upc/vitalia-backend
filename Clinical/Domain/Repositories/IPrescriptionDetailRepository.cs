using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Domain.Repositories;

public interface IPrescriptionDetailRepository : IBaseRepository<PrescriptionDetail>
{
    Task<IEnumerable<PrescriptionDetail>> FindAllByPrescriptionIdAsync(
        Guid prescriptionId,
        CancellationToken cancellationToken = default);

    Task<PrescriptionDetail?> FindByPrescriptionAndMedicineAsync(
        Guid prescriptionId,
        Guid medicineId,
        CancellationToken cancellationToken = default);
}
