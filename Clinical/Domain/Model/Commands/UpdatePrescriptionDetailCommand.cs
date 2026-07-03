namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdatePrescriptionDetailCommand(
    Guid PrescriptionId,
    Guid MedicineId,
    int Quantity,
    int Frequency,
    int Duration
);
