using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.CommandServices;

public class AvailabilitySlotCommandService(IAvailabilitySlotRepository availabilitySlotRepository, IUnitOfWork unitOfWork) : IAvailabilitySlotCommandService
{
    public async Task<AvailabilitySlot?> Handle(CreateAvailabilitySlotCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EAvailabilitySlotStatus>(command.Status, true, out var status))
            return null;

        var existsConflict = await availabilitySlotRepository.ExistsActiveSlotForDoctorAtAsync(
            command.DoctorId,
            command.Date,
            command.StartTime,
            cancellationToken: cancellationToken);

        if (existsConflict)
            return null;

        var slot = new AvailabilitySlot(
            command.Id,
            command.DoctorId,
            command.BranchId,
            command.Date,
            command.StartTime,
            command.EndTime,
            status);

        await availabilitySlotRepository.AddAsync(slot, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return slot;
    }

    public async Task<AvailabilitySlot?> Handle(UpdateAvailabilitySlotStatusCommand command, CancellationToken cancellationToken)
    {
        var slot = await availabilitySlotRepository.FindByPublicIdAsync(command.AvailabilitySlotId, cancellationToken);

        if (slot is null)
            return null;

        if (!Enum.TryParse<EAvailabilitySlotStatus>(command.Status, true, out var status))
            return null;

        slot.ChangeStatus(status);

        availabilitySlotRepository.Update(slot);
        await unitOfWork.CompleteAsync(cancellationToken);

        return slot;
    }

    public async Task<bool> Handle(DeleteAvailabilitySlotCommand command, CancellationToken cancellationToken)
    {
        var slot = await availabilitySlotRepository.FindByPublicIdAsync(command.AvailabilitySlotId, cancellationToken);

        if (slot is null)
            return false;

        availabilitySlotRepository.Remove(slot);
        await unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }
}