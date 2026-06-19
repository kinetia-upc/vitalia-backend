namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreatePrescriptionDetailResource(
    int PrescriptionId,
    int? MedicineId,
    string? MedicineName,
    int DoseAmount,
    string DoseUnit,
    string Frequency,
    string Duration
);
