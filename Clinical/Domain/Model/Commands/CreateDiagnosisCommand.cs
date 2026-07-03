namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateDiagnosisCommand(Guid MedicalRecordId, string Description);
