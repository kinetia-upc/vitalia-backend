using VitaliaBackend.Shared.Domain.Model;

namespace VitaliaBackend.Clinical.Domain.Model.Errors;

public static class ClinicalErrors
{
    public static readonly Error MedicalRecordCreationError =
        new("Clinical.MedicalRecordCreationError", "Error creating medical record");

    public static readonly Error MedicalRecordNotFoundError =
        new("Clinical.MedicalRecordNotFoundError", "Medical record not found");
}
