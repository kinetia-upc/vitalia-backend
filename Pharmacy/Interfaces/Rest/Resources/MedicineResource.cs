namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

public record MedicineResource(
    Guid Id,
    string Code,
    string Name,
    int UnitQuantity,
    string UnitType
);
