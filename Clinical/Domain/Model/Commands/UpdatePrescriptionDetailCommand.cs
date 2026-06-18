namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdatePrescriptionDetailCommand(
    int PrescriptionDetailId,
    int? MedicineId,
    string? MedicineName,
    int DoseAmount,
    string DoseUnit,
    string Frequency,
    string Duration
);
