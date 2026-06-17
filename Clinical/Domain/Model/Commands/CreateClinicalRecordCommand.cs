namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record CreateClinicalRecordCommand(
    string patientId,
    string appointmentId
);

    
