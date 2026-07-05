namespace VitaliaBackend.Clinical.Domain.Model;

public enum ClinicalError
{
    None,
    MedicalRecordNotFound,
    MedicalRecordAlreadyExistsForAppointment,
    MedicalRecordCodeGenerationFailed,
    InvalidMedicalRecordData,
    DiagnosisNotFound,
    InvalidDiagnosisDescription,
    InvalidDiagnosisCatalogCode,
    TreatmentNotFound,
    InvalidTreatmentDescription,
    PrescriptionNotFound,
    InvalidPrescriptionData,
    PrescriptionDetailNotFound,
    InvalidPrescriptionDetailData,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
