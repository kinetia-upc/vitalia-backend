namespace VitaliaBackend.Tenant.Domain.Model;

/// <summary>
///     Every error that can happen while creating, updating or deleting any of the
///     three Tenant aggregates (HealthcareCenter, Branch, AppointmentFee). One shared
///     enum per bounded context, the same pattern already used by SchedulingError for
///     Appointment/AvailabilitySlot.
/// </summary>
public enum TenantError
{
    None,
    HealthcareCenterCreationError,
    HealthcareCenterUpdateError,
    HealthcareCenterDeletionError,
    HealthcareCenterNotFoundError,
    BranchCreationError,
    BranchUpdateError,
    BranchDeletionError,
    BranchNotFoundError,
    AppointmentFeeCreationError,
    AppointmentFeeUpdateError,
    AppointmentFeeDeletionError,
    AppointmentFeeNotFoundError,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
