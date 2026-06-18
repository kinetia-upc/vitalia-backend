namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record PrescriptionResource(
    int Id,
    string MedicalRecordId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
