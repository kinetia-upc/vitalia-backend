using VitaliaBackend.Shared.Domain.Model;
namespace VitaliaBackend.Scheduling.Domain.Model.Errors;

public class SchedulingErrors
{
    public static readonly Error AppointmentCreationError = 
        new("Scheduling.AppointmentCreationError", "Error creating appointment");
    public static readonly Error AppointmentUpdateError = 
        new("Scheduling.AppointmentUpdateError", "Error updating appointment");
    public static readonly Error AppointmentDeletionError = 
        new("Scheduling.AppointmentDeletionError", "Error deleting appointment");
    public static readonly Error AppointmentNotFoundError = 
        new("Scheduling.AppointmentNotFoundError", "Appointment not found");
    
    public static readonly Error AvailabilitySlotCreationError = 
        new("Scheduling.AvailabilitySlotCreationError", "Error creating availability slot");
    public static readonly Error AvailabilitySlotUpdateError = 
        new("Scheduling.AvailabilitySlotUpdateError", "Error updating availability slot");
    public static readonly Error AvailabilitySlotDeletionError = 
        new("Scheduling.AvailabilitySlotDeletionError", "Error deleting availability slot");
    public static readonly Error AvailabilitySlotNotFoundError = 
        new("Scheduling.AvailabilitySlotNotFoundError", "Availability slot not found");
}