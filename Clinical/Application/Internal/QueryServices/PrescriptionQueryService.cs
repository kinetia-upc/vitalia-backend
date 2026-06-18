using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.QueryServices;

public class PrescriptionQueryService(IPrescriptionRepository prescriptionRepository) : IPrescriptionQueryService
{
    public async Task<Prescription?> Handle(GetPrescriptionByIdQuery query, CancellationToken cancellationToken)
    {
        return await prescriptionRepository.FindByIdAsync(query.PrescriptionId, cancellationToken);
    }
}
