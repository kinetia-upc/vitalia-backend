namespace VitaliaBackend.Pharmacy.Domain.Model.Commands;

public record CreateMedicineCommand(
    string Name,
    int UnitQuantity,
    string UnitType,
    decimal Price,
    int Stock
);
