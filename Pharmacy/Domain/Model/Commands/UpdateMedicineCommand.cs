namespace VitaliaBackend.Pharmacy.Domain.Model.Commands;

public record UpdateMedicineCommand(
    int MedicineId,
    string Name,
    int UnitQuantity,
    string UnitType,
    decimal Price,
    int Stock
);
