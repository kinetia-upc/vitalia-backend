using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Scheduling.Application.CommandServices;

public interface IAppointmentCommandService
{
    Task<Result<Appointment>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken);
    Task<Result<Appointment>> Handle(UpdateAppointmentScheduleCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteAppointmentCommand command, CancellationToken cancellationToken);
}