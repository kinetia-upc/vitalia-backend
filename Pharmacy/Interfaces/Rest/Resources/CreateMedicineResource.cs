namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

public record CreateMedicineResource(
    string Code,
    string Name,
    int UnitQuantity,
    string UnitType
);
