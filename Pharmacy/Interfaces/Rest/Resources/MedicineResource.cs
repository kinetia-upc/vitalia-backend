namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

public record MedicineResource(
    int Id,
    string Name,
    int UnitQuantity,
    string UnitType,
    decimal Price,
    int Stock
);
