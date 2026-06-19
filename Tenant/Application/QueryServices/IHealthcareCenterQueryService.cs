using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Queries;

namespace VitaliaBackend.Tenant.Application.QueryServices;

public interface IHealthcareCenterQueryService
{
    Task<HealthcareCenter?> Handle(GetHealthcareCenterByIdQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<HealthcareCenter>> Handle(GetHealthcareCentersQuery query, CancellationToken cancellationToken);
}
