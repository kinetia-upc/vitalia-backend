using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Commands;

namespace VitaliaBackend.Tenant.Application.CommandServices;

public interface IAppointmentFeeCommandService
{
    Task<Result<AppointmentFee>> Handle(CreateAppointmentFeeCommand command, CancellationToken cancellationToken);
    Task<Result<AppointmentFee>> Handle(UpdateAppointmentFeeCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteAppointmentFeeCommand command, CancellationToken cancellationToken);
}
