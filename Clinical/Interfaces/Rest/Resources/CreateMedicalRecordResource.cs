namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record CreateMedicalRecordResource(
    string PatientId,
    string AppointmentId
);
