namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record PrescriptionDetailResource(
    int Id,
    int PrescriptionId,
    int? MedicineId,
    string? MedicineName,
    int DoseAmount,
    string DoseUnit,
    string Frequency,
    string Duration,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
