using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.QueryServices;

public class PrescriptionDetailQueryService(IPrescriptionDetailRepository prescriptionDetailRepository)
    : IPrescriptionDetailQueryService
{
    public async Task<PrescriptionDetail?> Handle(
        GetPrescriptionDetailByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await prescriptionDetailRepository.FindByPrescriptionAndMedicineAsync(query.PrescriptionId, query.MedicineId, cancellationToken);
    }

    public async Task<IEnumerable<PrescriptionDetail>> Handle(
        GetPrescriptionDetailsByPrescriptionIdQuery query,
        CancellationToken cancellationToken)
    {
        return await prescriptionDetailRepository.FindAllByPrescriptionIdAsync(query.PrescriptionId, cancellationToken);
    }

    public async Task<IEnumerable<PrescriptionDetail>> Handle(
        GetAllPrescriptionDetailsQuery query,
        CancellationToken cancellationToken)
    {
        return await prescriptionDetailRepository.ListAsync(cancellationToken);
    }
}
