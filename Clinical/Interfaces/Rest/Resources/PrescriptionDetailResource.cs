namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record PrescriptionDetailResource(
    Guid PrescriptionId,
    Guid MedicineId,
    int Quantity,
    int Frequency,
    int Duration,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
