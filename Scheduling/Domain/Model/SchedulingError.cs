namespace VitaliaBackend.Scheduling.Domain.Model;

public enum SchedulingError
{
    None,
    AppointmentCreationError,
    AppointmentUpdateError,
    AppointmentDeletionError,
    AppointmentNotFoundError,
    AvailabilitySlotCreationError,
    AvailabilitySlotUpdateError,
    AvailabilitySlotDeletionError,
    AvailabilitySlotNotFoundError,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
