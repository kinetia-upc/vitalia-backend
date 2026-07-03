namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record DiagnosisResource(
    Guid Id,
    string Code,
    Guid MedicalRecordId,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
