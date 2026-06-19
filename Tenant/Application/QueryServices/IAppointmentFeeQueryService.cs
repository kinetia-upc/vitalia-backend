using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Queries;

namespace VitaliaBackend.Tenant.Application.QueryServices;

public interface IAppointmentFeeQueryService
{
    Task<AppointmentFee?> Handle(GetAppointmentFeeByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<AppointmentFee>> Handle(GetAppointmentFeesQuery query, CancellationToken cancellationToken);
}
