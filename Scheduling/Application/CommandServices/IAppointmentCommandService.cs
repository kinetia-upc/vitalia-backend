using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Commands;

namespace VitaliaBackend.Scheduling.Application.CommandServices;

public interface IAppointmentCommandService
{
    Task<Appointment?> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken);
    Task<Appointment?> Handle(UpdateAppointmentScheduleCommand command, CancellationToken cancellationToken);
    Task<bool> Handle(DeleteAppointmentCommand command, CancellationToken cancellationToken);
}