namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;

public record UpdateMedicineResource(
    string Name,
    int UnitQuantity,
    string UnitType,
    decimal Price,
    int Stock
);
