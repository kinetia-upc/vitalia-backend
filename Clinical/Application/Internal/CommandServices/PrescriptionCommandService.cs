using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class PrescriptionCommandService(
    IPrescriptionRepository prescriptionRepository,
    IPrescriptionDetailRepository prescriptionDetailRepository,
    IMedicalRecordRepository medicalRecordRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IPrescriptionCommandService
{
    public async Task<Result<Prescription>> Handle(CreatePrescriptionCommand command, CancellationToken cancellationToken)
    {
        if (command.MedicalRecordId == Guid.Empty)
            return Result<Prescription>.Failure(
                ClinicalError.InvalidPrescriptionData,
                localizer[nameof(ClinicalError.InvalidPrescriptionData)]);

        var medicalRecord = await medicalRecordRepository.FindByIdAsync(command.MedicalRecordId, cancellationToken);

        if (medicalRecord is null)
            return Result<Prescription>.Failure(
                ClinicalError.MedicalRecordNotFound,
                localizer[nameof(ClinicalError.MedicalRecordNotFound)]);

        const int maxCodeGenerationAttempts = 5;
        var nextCodeNumber = await prescriptionRepository.GetMaxCodeNumberAsync("prc-", cancellationToken) + 1;

        for (var attempt = 0; attempt < maxCodeGenerationAttempts; attempt++)
        {
            var candidateCode = $"prc-{nextCodeNumber + attempt:D5}";
            var codeAlreadyExists = await prescriptionRepository.ExistsByCodeAsync(candidateCode, cancellationToken);

            if (codeAlreadyExists)
                continue;

            var prescription = new Prescription(Guid.Empty, candidateCode, command.MedicalRecordId);

            try
            {
                await prescriptionRepository.AddAsync(prescription, cancellationToken);
                medicalRecordRepository.Update(medicalRecord);
                await unitOfWork.CompleteAsync(cancellationToken);
                return Result<Prescription>.Success(prescription);
            }
            catch (OperationCanceledException)
            {
                return Result<Prescription>.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
            }
            catch (DbUpdateException)
            {
                return Result<Prescription>.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
            }
            catch (Exception)
            {
                return Result<Prescription>.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
            }
        }

        return Result<Prescription>.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
    }

    public async Task<Result> Handle(DeletePrescriptionCommand command, CancellationToken cancellationToken)
    {
        var prescription = await prescriptionRepository.FindByIdAsync(command.PrescriptionId, cancellationToken);

        if (prescription is null)
            return Result.Failure(
                ClinicalError.PrescriptionNotFound,
                localizer[nameof(ClinicalError.PrescriptionNotFound)]);

        var prescriptionDetails = await prescriptionDetailRepository.FindAllByPrescriptionIdAsync(
            command.PrescriptionId,
            cancellationToken);
        var medicalRecord = await medicalRecordRepository.FindByIdAsync(prescription.MedicalRecordId, cancellationToken);

        try
        {
            foreach (var prescriptionDetail in prescriptionDetails)
                prescriptionDetailRepository.Remove(prescriptionDetail);

            prescriptionRepository.Remove(prescription);
            if (medicalRecord is not null) medicalRecordRepository.Update(medicalRecord);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }
}
