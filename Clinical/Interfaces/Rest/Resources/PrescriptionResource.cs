namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record PrescriptionResource(
    Guid Id,
    string Code,
    Guid MedicalRecordId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
