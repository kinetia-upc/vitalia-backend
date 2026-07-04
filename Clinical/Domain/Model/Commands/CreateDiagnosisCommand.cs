namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateDiagnosisCommand(Guid MedicalRecordId, string Cie10Code, string Description);
