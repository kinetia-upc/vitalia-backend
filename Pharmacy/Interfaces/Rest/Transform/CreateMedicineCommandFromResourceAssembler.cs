using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Transform;

public static class CreateMedicineCommandFromResourceAssembler
{
    public static CreateMedicineCommand ToCommandFromResource(CreateMedicineResource resource)
    {
        return new CreateMedicineCommand(
            resource.Code,
            resource.Name,
            resource.UnitQuantity,
            resource.UnitType);
    }
}
