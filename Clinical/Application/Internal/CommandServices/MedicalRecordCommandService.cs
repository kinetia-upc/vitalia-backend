using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class MedicalRecordCommandService(
    IMedicalRecordRepository medicalRecordRepository,
    IAppointmentRepository appointmentRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IMedicalRecordCommandService
{
    public async Task<Result<MedicalRecord>> Handle(
        CreateClinicalRecordCommand command,
        CancellationToken cancellationToken)
    {
        if (command.AppointmentId == Guid.Empty)
            return Result<MedicalRecord>.Failure(
                ClinicalError.InvalidMedicalRecordData,
                localizer[nameof(ClinicalError.InvalidMedicalRecordData)]);

        var appointment = await appointmentRepository.FindByIdAsync(command.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result<MedicalRecord>.Failure(
                ClinicalError.InvalidMedicalRecordData,
                localizer[nameof(ClinicalError.InvalidMedicalRecordData)]);

        var existsForAppointment = await medicalRecordRepository.ExistsByAppointmentIdAsync(
            command.AppointmentId,
            cancellationToken);

        if (existsForAppointment)
            return Result<MedicalRecord>.Failure(
                ClinicalError.MedicalRecordAlreadyExistsForAppointment,
                localizer[nameof(ClinicalError.MedicalRecordAlreadyExistsForAppointment)]);

        const int maxCodeGenerationAttempts = 5;

        for (var attempt = 0; attempt < maxCodeGenerationAttempts; attempt++)
        {
            var medicalRecord = new MedicalRecord(command.AppointmentId, appointment.PatientId);
            var codeAlreadyExists = await medicalRecordRepository.ExistsByCodeAsync(
                medicalRecord.Code,
                cancellationToken);

            if (codeAlreadyExists)
                continue;

            try
            {
                await medicalRecordRepository.AddAsync(medicalRecord, cancellationToken);
                await unitOfWork.CompleteAsync(cancellationToken);

                return Result<MedicalRecord>.Success(medicalRecord);
            }
            catch (OperationCanceledException)
            {
                return Result<MedicalRecord>.Failure(
                    ClinicalError.OperationCancelled,
                    localizer[nameof(ClinicalError.OperationCancelled)]);
            }
            catch (DbUpdateException)
            {
                return Result<MedicalRecord>.Failure(
                    ClinicalError.DatabaseError,
                    localizer[nameof(ClinicalError.DatabaseError)]);
            }
            catch (Exception)
            {
                return Result<MedicalRecord>.Failure(
                    ClinicalError.InternalServerError,
                    localizer[nameof(ClinicalError.InternalServerError)]);
            }
        }

        return Result<MedicalRecord>.Failure(
            ClinicalError.MedicalRecordCodeGenerationFailed,
            localizer[nameof(ClinicalError.MedicalRecordCodeGenerationFailed)]);
    }
}
