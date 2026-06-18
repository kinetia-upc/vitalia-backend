namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreatePrescriptionDetailCommand(
    int PrescriptionId,
    int? MedicineId,
    string? MedicineName,
    int DoseAmount,
    string DoseUnit,
    string Frequency,
    string Duration
);
