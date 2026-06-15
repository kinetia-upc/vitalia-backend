using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;

namespace VitaliaBackend.Scheduling.Application.CommandServices;

public interface IAvailabilitySlotCommandService
{
    Task<AvailabilitySlot?> Handle(CreateAvailabilitySlotCommand command, CancellationToken cancellationToken);
    Task<AvailabilitySlot?> Handle(UpdateAvailabilitySlotStatusCommand command, CancellationToken cancellationToken);
    Task<bool> Handle(DeleteAvailabilitySlotCommand command, CancellationToken cancellationToken);
}