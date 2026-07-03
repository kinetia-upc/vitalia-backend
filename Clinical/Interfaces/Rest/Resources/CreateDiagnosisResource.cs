namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreateDiagnosisResource(string Code, Guid MedicalRecordId, string Description);
