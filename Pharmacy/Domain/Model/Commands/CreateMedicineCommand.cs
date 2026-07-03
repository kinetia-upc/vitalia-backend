namespace VitaliaBackend.Pharmacy.Domain.Model.Commands;

public record CreateMedicineCommand(
    string Code,
    string Name,
    int UnitQuantity,
    string UnitType
);
