using VitaliaBackend.Tenant.Application.QueryServices;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Queries;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Application.Internal.QueryServices;

public class AppointmentFeeQueryService(IAppointmentFeeRepository appointmentFeeRepository)
    : IAppointmentFeeQueryService
{
    public async Task<AppointmentFee?> Handle(GetAppointmentFeeByIdQuery query, CancellationToken cancellationToken)
    {
        return await appointmentFeeRepository.FindByPublicIdAsync(query.AppointmentFeeId, cancellationToken);
    }

    public async Task<IEnumerable<AppointmentFee>> Handle(GetAppointmentFeesQuery query, CancellationToken cancellationToken)
    {
        return await appointmentFeeRepository.SearchAsync(query.BranchId, cancellationToken);
    }
}
