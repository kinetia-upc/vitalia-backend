using VitaliaBackend.Tenant.Application.QueryServices;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Queries;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Application.Internal.QueryServices;

public class HealthcareCenterQueryService(IHealthcareCenterRepository healthcareCenterRepository)
    : IHealthcareCenterQueryService
{
    public async Task<HealthcareCenter?> Handle(GetHealthcareCenterByIdQuery query, CancellationToken cancellationToken)
    {
        return await healthcareCenterRepository.FindByPublicIdAsync(query.HealthcareCenterId, cancellationToken);
    }

    public async Task<IEnumerable<HealthcareCenter>> Handle(GetHealthcareCentersQuery query, CancellationToken cancellationToken)
    {
        return await healthcareCenterRepository.ListAsync(cancellationToken);
    }
}
