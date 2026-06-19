using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Domain.Model;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Scheduling.Application.Internal.CommandServices;

public class AvailabilitySlotCommandService(
    IAvailabilitySlotRepository availabilitySlotRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IAvailabilitySlotCommandService
{
    public async Task<Result<AvailabilitySlot>> Handle(CreateAvailabilitySlotCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EAvailabilitySlotStatus>(command.Status, true, out var status))
            return Result<AvailabilitySlot>.Failure(
                SchedulingError.AvailabilitySlotCreationError,
                localizer[nameof(SchedulingError.AvailabilitySlotCreationError)]);

        var existsConflict = await availabilitySlotRepository.ExistsActiveSlotForDoctorAtAsync(
            command.DoctorId,
            command.Date,
            command.StartTime,
            cancellationToken: cancellationToken);

        if (existsConflict)
            return Result<AvailabilitySlot>.Failure(
                SchedulingError.AvailabilitySlotCreationError,
                localizer[nameof(SchedulingError.AvailabilitySlotCreationError)]);

        var slot = new AvailabilitySlot(
            command.Id,
            command.DoctorId,
            command.BranchId,
            command.Date,
            command.StartTime,
            command.EndTime,
            status);

        try
        {
            await availabilitySlotRepository.AddAsync(slot, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<AvailabilitySlot>.Success(slot);
        }
        catch (OperationCanceledException)
        {
            return Result<AvailabilitySlot>.Failure(SchedulingError.OperationCancelled, localizer[nameof(SchedulingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<AvailabilitySlot>.Failure(SchedulingError.DatabaseError, localizer[nameof(SchedulingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<AvailabilitySlot>.Failure(SchedulingError.InternalServerError, localizer[nameof(SchedulingError.InternalServerError)]);
        }
    }

    public async Task<Result<AvailabilitySlot>> Handle(UpdateAvailabilitySlotStatusCommand command, CancellationToken cancellationToken)
    {
        var slot = await availabilitySlotRepository.FindByPublicIdAsync(command.AvailabilitySlotId, cancellationToken);

        if (slot is null)
            return Result<AvailabilitySlot>.Failure(
                SchedulingError.AvailabilitySlotNotFoundError,
                localizer[nameof(SchedulingError.AvailabilitySlotNotFoundError)]);

        if (!Enum.TryParse<EAvailabilitySlotStatus>(command.Status, true, out var status))
            return Result<AvailabilitySlot>.Failure(
                SchedulingError.AvailabilitySlotUpdateError,
                localizer[nameof(SchedulingError.AvailabilitySlotUpdateError)]);

        slot.ChangeStatus(status);

        try
        {
            availabilitySlotRepository.Update(slot);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<AvailabilitySlot>.Success(slot);
        }
        catch (OperationCanceledException)
        {
            return Result<AvailabilitySlot>.Failure(SchedulingError.OperationCancelled, localizer[nameof(SchedulingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<AvailabilitySlot>.Failure(SchedulingError.DatabaseError, localizer[nameof(SchedulingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<AvailabilitySlot>.Failure(SchedulingError.InternalServerError, localizer[nameof(SchedulingError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteAvailabilitySlotCommand command, CancellationToken cancellationToken)
    {
        var slot = await availabilitySlotRepository.FindByPublicIdAsync(command.AvailabilitySlotId, cancellationToken);

        if (slot is null)
            return Result.Failure(
                SchedulingError.AvailabilitySlotNotFoundError,
                localizer[nameof(SchedulingError.AvailabilitySlotNotFoundError)]);

        try
        {
            availabilitySlotRepository.Remove(slot);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(SchedulingError.OperationCancelled, localizer[nameof(SchedulingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(SchedulingError.DatabaseError, localizer[nameof(SchedulingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result.Failure(SchedulingError.InternalServerError, localizer[nameof(SchedulingError.InternalServerError)]);
        }
    }
}