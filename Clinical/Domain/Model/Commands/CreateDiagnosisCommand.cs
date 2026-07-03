namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateDiagnosisCommand(string Code, Guid MedicalRecordId, string Description);
