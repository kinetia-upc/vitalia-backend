using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class MedicalRecordCommandService(
    IMedicalRecordRepository medicalRecordRepository,
    IUnitOfWork unitOfWork)
    : IMedicalRecordCommandService
{
    public async Task<MedicalRecord?> Handle(
        CreateClinicalRecordCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsValid(command.patientId, command.appointmentId))
            return null;

        var existsForAppointment = await medicalRecordRepository.ExistsByAppointmentIdAsync(
            command.appointmentId,
            cancellationToken);

        if (existsForAppointment)
            return null;

        const int maxCodeGenerationAttempts = 5;

        for (var attempt = 0; attempt < maxCodeGenerationAttempts; attempt++)
        {
            var medicalRecord = new MedicalRecord(command.appointmentId, command.patientId);
            var codeAlreadyExists = await medicalRecordRepository.ExistsByCodeAsync(
                medicalRecord.Code,
                cancellationToken);

            if (codeAlreadyExists)
                continue;

            await medicalRecordRepository.AddAsync(medicalRecord, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return medicalRecord;
        }

        return null;
    }

    private static bool IsValid(string patientId, string appointmentId)
    {
        return !string.IsNullOrWhiteSpace(patientId)
               && !string.IsNullOrWhiteSpace(appointmentId);
    }
}
