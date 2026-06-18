using VitaliaBackend.Shared.Domain.Model;

namespace VitaliaBackend.Clinical.Domain.Model.Errors;

public static class ClinicalErrors
{
    public static readonly Error MedicalRecordCreationError =
        new("Clinical.MedicalRecordCreationError", "Error creating medical record");

    public static readonly Error MedicalRecordNotFoundError =
        new("Clinical.MedicalRecordNotFoundError", "Medical record not found");

    public static readonly Error DiagnosisCreationError =
        new("Clinical.DiagnosisCreationError", "Error creating diagnosis");

    public static readonly Error DiagnosisNotFoundError =
        new("Clinical.DiagnosisNotFoundError", "Diagnosis not found");

    public static readonly Error TreatmentCreationError =
        new("Clinical.TreatmentCreationError", "Error creating treatment");

    public static readonly Error TreatmentNotFoundError =
        new("Clinical.TreatmentNotFoundError", "Treatment not found");

    public static readonly Error PrescriptionCreationError =
        new("Clinical.PrescriptionCreationError", "Error creating prescription");

    public static readonly Error PrescriptionNotFoundError =
        new("Clinical.PrescriptionNotFoundError", "Prescription not found");

    public static readonly Error PrescriptionDetailCreationError =
        new("Clinical.PrescriptionDetailCreationError", "Error creating prescription detail");

    public static readonly Error PrescriptionDetailNotFoundError =
        new("Clinical.PrescriptionDetailNotFoundError", "Prescription detail not found");
}
