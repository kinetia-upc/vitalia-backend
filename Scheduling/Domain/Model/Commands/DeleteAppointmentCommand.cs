namespace VitaliaBackend.Scheduling.Domain.Model.Commands;

public record DeleteAppointmentCommand(
    string AppointmentId
    );