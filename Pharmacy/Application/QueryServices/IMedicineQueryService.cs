using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Model.Queries;

namespace VitaliaBackend.Pharmacy.Application.QueryServices;

public interface IMedicineQueryService
{
    Task<Medicine?> Handle(GetMedicineByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<Medicine>> Handle(GetMedicinesQuery query, CancellationToken cancellationToken);
}
