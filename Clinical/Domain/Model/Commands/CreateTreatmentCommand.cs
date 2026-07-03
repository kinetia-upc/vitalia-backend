namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateTreatmentCommand(Guid MedicalRecordId, string Description);
