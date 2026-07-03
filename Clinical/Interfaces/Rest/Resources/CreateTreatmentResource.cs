namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreateTreatmentResource(Guid MedicalRecordId, string Description);
