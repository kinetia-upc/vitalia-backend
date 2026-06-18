namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record DiagnosisResource(
    int Id,
    string MedicalRecordId,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
