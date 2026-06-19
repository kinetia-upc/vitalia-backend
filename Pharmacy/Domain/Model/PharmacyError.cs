namespace VitaliaBackend.Pharmacy.Domain.Model;

public enum PharmacyError
{
    None,
    MedicineCreationError,
    MedicineUpdateError,
    MedicineDeletionError,
    MedicineNotFoundError,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
