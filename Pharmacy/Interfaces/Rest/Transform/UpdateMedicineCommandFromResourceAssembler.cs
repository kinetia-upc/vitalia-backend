using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Transform;

public static class UpdateMedicineCommandFromResourceAssembler
{
    public static UpdateMedicineCommand ToCommandFromResource(int medicineId, UpdateMedicineResource resource)
    {
        return new UpdateMedicineCommand(
            medicineId,
            resource.Name,
            resource.UnitQuantity,
            resource.UnitType,
            resource.Price,
            resource.Stock);
    }
}
