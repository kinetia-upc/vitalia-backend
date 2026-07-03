namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreateDiagnosisResource(Guid MedicalRecordId, string Description);
