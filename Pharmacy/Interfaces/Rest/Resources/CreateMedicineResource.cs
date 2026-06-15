namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

public record CreateMedicineResource(
    string Name,
    int UnitQuantity,
    string UnitType,
    decimal Price,
    int Stock
);
