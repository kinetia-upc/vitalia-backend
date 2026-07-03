namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreateTreatmentResource(string Code, Guid MedicalRecordId, string Description);
