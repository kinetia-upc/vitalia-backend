namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record DiagnosisResource(
    int Id,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
