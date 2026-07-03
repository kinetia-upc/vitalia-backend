namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateTreatmentCommand(string Code, Guid MedicalRecordId, string Description);
