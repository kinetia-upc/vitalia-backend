namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record MedicalRecordResource(
    int Id,
    string Code,
    string PatientId,
    string AppointmentId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
