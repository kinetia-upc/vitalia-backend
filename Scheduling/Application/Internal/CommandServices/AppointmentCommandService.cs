using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.CommandServices;

public class AppointmentCommandService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork) : IAppointmentCommandService
{
    public async Task<Appointment?> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<EAppointmentStatus>(command.Status, true, out var status))
            return null;

        if (!Enum.TryParse<EPaymentStatus>(command.PaymentStatus, true, out var paymentStatus))
            return null;

        var existsConflict = await appointmentRepository.ExistsActiveAppointmentForDoctorAtAsync(
            command.DoctorId,
            command.ScheduledAt,
            cancellationToken: cancellationToken);

        if (existsConflict)
            return null;

        var appointment = new Appointment(
            command.Id,
            command.DoctorId,
            command.PatientId,
            command.BranchId,
            command.ScheduledAt,
            command.Reason,
            status,
            paymentStatus);

        await appointmentRepository.AddAsync(appointment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return appointment;
    }

    public async Task<Appointment?> Handle(UpdateAppointmentScheduleCommand command, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.FindByPublicIdAsync(command.AppointmentId, cancellationToken);

        if (appointment is null)
            return null;

        var existsConflict = await appointmentRepository.ExistsActiveAppointmentForDoctorAtAsync(
            command.DoctorId,
            command.ScheduledAt,
            command.AppointmentId,
            cancellationToken);

        if (existsConflict)
            return null;

        appointment.Reschedule(
            command.DoctorId,
            command.PatientId,
            command.BranchId,
            command.ScheduledAt,
            command.Reason);

        if (!string.IsNullOrWhiteSpace(command.Status))
        {
            if (!Enum.TryParse<EAppointmentStatus>(command.Status, true, out var status))
                return null;

            appointment.ChangeStatus(status);
        }
        
        if (!string.IsNullOrWhiteSpace(command.PaymentStatus))
        {
            if (!Enum.TryParse<EPaymentStatus>(command.PaymentStatus, true, out var paymentStatus))
                return null;

            appointment.ChangePaymentStatus(paymentStatus);
        }

        appointmentRepository.Update(appointment);
        await unitOfWork.CompleteAsync(cancellationToken);

        return appointment;
    }
    
    public async Task<bool> Handle(DeleteAppointmentCommand command, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.FindByPublicIdAsync(command.AppointmentId, cancellationToken);

        if (appointment is null)
            return false;

        appointmentRepository.Remove(appointment);
        await unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }
}