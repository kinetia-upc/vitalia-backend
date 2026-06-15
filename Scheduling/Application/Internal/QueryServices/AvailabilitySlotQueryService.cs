using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.QueryServices;

public class AvailabilitySlotQueryService(IAvailabilitySlotRepository availabilitySlotRepository) : IAvailabilitySlotQueryService
{
    public async Task<AvailabilitySlot?> Handle(GetAvailabilitySlotByIdQuery query, CancellationToken cancellationToken)
    {
        return await availabilitySlotRepository.FindByPublicIdAsync(query.AvailabilitySlotId, cancellationToken);
    }

    public async Task<IEnumerable<AvailabilitySlot>> Handle(GetAvailabilitySlotsQuery query, CancellationToken cancellationToken)
    {
        return await availabilitySlotRepository.SearchAsync(
            query.DoctorId,
            query.BranchId,
            query.Date,
            cancellationToken);
    }
}