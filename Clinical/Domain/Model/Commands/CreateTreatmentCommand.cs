namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateTreatmentCommand(string MedicalRecordId, string Description);
