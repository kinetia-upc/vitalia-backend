using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Commands;

namespace VitaliaBackend.Tenant.Application.CommandServices;

public interface IHealthcareCenterCommandService
{
    Task<Result<HealthcareCenter>> Handle(CreateHealthcareCenterCommand command, CancellationToken cancellationToken);
    Task<Result<HealthcareCenter>> Handle(UpdateHealthcareCenterCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteHealthcareCenterCommand command, CancellationToken cancellationToken);
}
