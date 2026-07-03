namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreatePrescriptionDetailCommand(
    Guid PrescriptionId,
    Guid MedicineId,
    int Quantity,
    int Frequency,
    int Duration
);
