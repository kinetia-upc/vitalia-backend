using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Scheduling.Application.CommandServices;

public interface IAvailabilitySlotCommandService
{
    Task<Result<AvailabilitySlot>> Handle(CreateAvailabilitySlotCommand command, CancellationToken cancellationToken);
    Task<Result<AvailabilitySlot>> Handle(UpdateAvailabilitySlotStatusCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteAvailabilitySlotCommand command, CancellationToken cancellationToken);
}