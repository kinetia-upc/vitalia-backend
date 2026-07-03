namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

public record UpdateMedicineResource(
    string Code,
    string Name,
    int UnitQuantity,
    string UnitType
);
