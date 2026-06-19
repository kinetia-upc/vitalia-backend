namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record UpdatePrescriptionDetailResource(
    int? MedicineId,
    string? MedicineName,
    int DoseAmount,
    string DoseUnit,
    string Frequency,
    string Duration
);
