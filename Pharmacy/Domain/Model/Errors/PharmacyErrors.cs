using VitaliaBackend.Shared.Domain.Model;

namespace VitaliaBackend.Pharmacy.Domain.Model.Errors;

public static class PharmacyErrors
{
    public static readonly Error MedicineCreationError =
        new("Pharmacy.MedicineCreationError", "Error creating medicine");

    public static readonly Error MedicineUpdateError =
        new("Pharmacy.MedicineUpdateError", "Error updating medicine");

    public static readonly Error MedicineDeletionError =
        new("Pharmacy.MedicineDeletionError", "Error deleting medicine");

    public static readonly Error MedicineNotFoundError =
        new("Pharmacy.MedicineNotFoundError", "Medicine not found");
}
