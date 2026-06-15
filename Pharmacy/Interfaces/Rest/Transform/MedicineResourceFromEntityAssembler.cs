using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Transform;

public static class MedicineResourceFromEntityAssembler
{
    public static MedicineResource ToResourceFromEntity(Medicine entity)
    {
        return new MedicineResource(
            entity.Id,
            entity.Name,
            entity.UnitQuantity,
            entity.UnitType,
            entity.Price,
            entity.Stock);
    }
}
