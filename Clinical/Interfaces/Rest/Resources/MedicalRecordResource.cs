namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record MedicalRecordResource(
    Guid Id,
    string Code,
    Guid PatientId,
    Guid AppointmentId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
