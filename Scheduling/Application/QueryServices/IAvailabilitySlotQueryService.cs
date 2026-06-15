using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Queries;

namespace VitaliaBackend.Scheduling.Application.QueryServices;

public interface IAvailabilitySlotQueryService
{
    Task<AvailabilitySlot?> Handle(GetAvailabilitySlotByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<AvailabilitySlot>> Handle(GetAvailabilitySlotsQuery query, CancellationToken cancellationToken);
}