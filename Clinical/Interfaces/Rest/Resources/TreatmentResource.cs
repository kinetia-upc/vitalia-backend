namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record TreatmentResource(
    int Id,
    string Description,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
