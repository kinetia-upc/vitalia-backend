namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record TreatmentResource(
    Guid Id,
    string Code,
    Guid MedicalRecordId,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
