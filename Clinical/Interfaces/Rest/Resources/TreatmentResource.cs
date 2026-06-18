namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record TreatmentResource(
    int Id,
    string MedicalRecordId,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
