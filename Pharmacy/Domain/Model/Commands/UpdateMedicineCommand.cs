namespace VitaliaBackend.Pharmacy.Domain.Model.Commands;

public record UpdateMedicineCommand(
    Guid MedicineId,
    string Code,
    string Name,
    int UnitQuantity,
    string UnitType
);
