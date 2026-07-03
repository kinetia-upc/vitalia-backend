namespace VitaliaBackend.Clinical.Domain.Model.Queries;

public record GetPrescriptionsByMedicalRecordIdQuery(Guid MedicalRecordId);
