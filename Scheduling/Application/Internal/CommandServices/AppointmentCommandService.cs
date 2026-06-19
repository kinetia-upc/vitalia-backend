using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Domain.Model;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Scheduling.Application.Internal.CommandServices;

public class AppointmentCommandService(
    IAppointmentRepository appointmentRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IAppointmentCommandService
{
    public async Task<Result<Appointment>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EAppointmentStatus>(command.Status, true, out var status))
            return Result<Appointment>.Failure(
                SchedulingError.AppointmentCreationError,
                localizer[nameof(SchedulingError.AppointmentCreationError)]);

        if (!Enum.TryParse<EPaymentStatus>(command.PaymentStatus, true, out var paymentStatus))
            return Result<Appointment>.Failure(
                SchedulingError.AppointmentCreationError,
                localizer[nameof(SchedulingError.AppointmentCreationError)]);

        var existsConflict = await appointmentRepository.ExistsActiveAppointmentForDoctorAtAsync(
            command.DoctorId,
            command.ScheduledAt,
            cancellationToken: cancellationToken);

        if (existsConflict)
            return Result<Appointment>.Failure(
                SchedulingError.AppointmentCreationError,
                localizer[nameof(SchedulingError.AppointmentCreationError)]);

        var appointment = new Appointment(
            command.Id,
            command.DoctorId,
            command.PatientId,
            command.BranchId,
            command.ScheduledAt,
            command.Reason,
            status,
            paymentStatus);

        try
        {
            await appointmentRepository.AddAsync(appointment, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Appointment>.Success(appointment);
        }
        catch (OperationCanceledException)
        {
            return Result<Appointment>.Failure(SchedulingError.OperationCancelled, localizer[nameof(SchedulingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Appointment>.Failure(SchedulingError.DatabaseError, localizer[nameof(SchedulingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Appointment>.Failure(SchedulingError.InternalServerError, localizer[nameof(SchedulingError.InternalServerError)]);
        }
    }

    public async Task<Result<Appointment>> Handle(UpdateAppointmentScheduleCommand command, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.FindByPublicIdAsync(command.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result<Appointment>.Failure(
                SchedulingError.AppointmentNotFoundError,
                localizer[nameof(SchedulingError.AppointmentNotFoundError)]);

        var existsConflict = await appointmentRepository.ExistsActiveAppointmentForDoctorAtAsync(
            command.DoctorId,
            command.ScheduledAt,
            command.AppointmentId,
            cancellationToken);

        if (existsConflict)
            return Result<Appointment>.Failure(
                SchedulingError.AppointmentUpdateError,
                localizer[nameof(SchedulingError.AppointmentUpdateError)]);

        appointment.Reschedule(
            command.DoctorId,
            command.PatientId,
            command.BranchId,
            command.ScheduledAt,
            command.Reason);

        if (!string.IsNullOrWhiteSpace(command.Status))
        {
            if (!Enum.TryParse<EAppointmentStatus>(command.Status, true, out var status))
                return Result<Appointment>.Failure(
                    SchedulingError.AppointmentUpdateError,
                    localizer[nameof(SchedulingError.AppointmentUpdateError)]);

            appointment.ChangeStatus(status);
        }
        
        if (!string.IsNullOrWhiteSpace(command.PaymentStatus))
        {
            if (!Enum.TryParse<EPaymentStatus>(command.PaymentStatus, true, out var paymentStatus))
                return Result<Appointment>.Failure(
                    SchedulingError.AppointmentUpdateError,
                    localizer[nameof(SchedulingError.AppointmentUpdateError)]);

            appointment.ChangePaymentStatus(paymentStatus);
        }

        try
        {
            appointmentRepository.Update(appointment);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Appointment>.Success(appointment);
        }
        catch (OperationCanceledException)
        {
            return Result<Appointment>.Failure(SchedulingError.OperationCancelled, localizer[nameof(SchedulingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Appointment>.Failure(SchedulingError.DatabaseError, localizer[nameof(SchedulingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Appointment>.Failure(SchedulingError.InternalServerError, localizer[nameof(SchedulingError.InternalServerError)]);
        }
    }
    
    public async Task<Result> Handle(DeleteAppointmentCommand command, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.FindByPublicIdAsync(command.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(
                SchedulingError.AppointmentNotFoundError,
                localizer[nameof(SchedulingError.AppointmentNotFoundError)]);

        try
        {
            appointmentRepository.Remove(appointment);
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