namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreatePrescriptionDetailResource(
    Guid PrescriptionId,
    Guid MedicineId,
    int Quantity,
    int Frequency,
    int Duration
);
