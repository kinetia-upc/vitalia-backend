using VitaliaBackend.Pharmacy.Application.QueryServices;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Model.Queries;
using VitaliaBackend.Pharmacy.Domain.Repositories;

namespace VitaliaBackend.Pharmacy.Application.Internal.QueryServices;

public class MedicineQueryService(IMedicineRepository medicineRepository) : IMedicineQueryService
{
    public async Task<Medicine?> Handle(GetMedicineByIdQuery query, CancellationToken cancellationToken)
    {
        return await medicineRepository.FindByIdAsync(query.MedicineId, cancellationToken);
    }

    public async Task<IEnumerable<Medicine>> Handle(GetMedicinesQuery query, CancellationToken cancellationToken)
    {
        return await medicineRepository.SearchAsync(query.Search, cancellationToken);
    }
}
