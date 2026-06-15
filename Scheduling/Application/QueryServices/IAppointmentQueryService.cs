using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Queries;

namespace VitaliaBackend.Scheduling.Application.QueryServices;

public interface IAppointmentQueryService
{
    Task<Appointment?> Handle(GetAppointmentByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<Appointment>> Handle(GetAppointmentsQuery query, CancellationToken cancellationToken);
}