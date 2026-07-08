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

public class TreatmentCommandService(
    ITreatmentRepository treatmentRepository,
    IMedicalRecordRepository medicalRecordRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : ITreatmentCommandService
{
    public async Task<Result<Treatment>> Handle(CreateTreatmentCommand command, CancellationToken cancellationToken)
    {
        if (command.MedicalRecordId == Guid.Empty || !HasValidDescription(command.Description))
            return Result<Treatment>.Failure(
                ClinicalError.InvalidTreatmentDescription,
                localizer[nameof(ClinicalError.InvalidTreatmentDescription)]);

        var medicalRecord = await medicalRecordRepository.FindByIdAsync(command.MedicalRecordId, cancellationToken);

        if (medicalRecord is null)
            return Result<Treatment>.Failure(
                ClinicalError.MedicalRecordNotFound,
                localizer[nameof(ClinicalError.MedicalRecordNotFound)]);

        var treatment = new Treatment(Guid.NewGuid(), GenerateCode(), command.MedicalRecordId, command.Description);

        try
        {
            await treatmentRepository.AddAsync(treatment, cancellationToken);
            medicalRecordRepository.Update(medicalRecord);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Treatment>.Success(treatment);
        }
        catch (OperationCanceledException)
        {
            return Result<Treatment>.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Treatment>.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Treatment>.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    public async Task<Result<Treatment>> Handle(
        UpdateTreatmentDescriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (!HasValidDescription(command.Description))
            return Result<Treatment>.Failure(
                ClinicalError.InvalidTreatmentDescription,
                localizer[nameof(ClinicalError.InvalidTreatmentDescription)]);

        var treatment = await treatmentRepository.FindByIdAsync(command.TreatmentId, cancellationToken);

        if (treatment is null)
            return Result<Treatment>.Failure(
                ClinicalError.TreatmentNotFound,
                localizer[nameof(ClinicalError.TreatmentNotFound)]);

        treatment.UpdateDescription(command.Description);
        var medicalRecord = await medicalRecordRepository.FindByIdAsync(treatment.MedicalRecordId, cancellationToken);

        try
        {
            treatmentRepository.Update(treatment);
            if (medicalRecord is not null) medicalRecordRepository.Update(medicalRecord);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Treatment>.Success(treatment);
        }
        catch (OperationCanceledException)
        {
            return Result<Treatment>.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Treatment>.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Treatment>.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteTreatmentCommand command, CancellationToken cancellationToken)
    {
        var treatment = await treatmentRepository.FindByIdAsync(command.TreatmentId, cancellationToken);

        if (treatment is null)
            return Result.Failure(
                ClinicalError.TreatmentNotFound,
                localizer[nameof(ClinicalError.TreatmentNotFound)]);

        var medicalRecord = await medicalRecordRepository.FindByIdAsync(treatment.MedicalRecordId, cancellationToken);

        try
        {
            treatmentRepository.Remove(treatment);
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

    private static bool HasValidDescription(string description)
    {
        return !string.IsNullOrWhiteSpace(description) && description.Trim().Length >= 5;
    }

    private static string GenerateCode()
    {
        return $"trt-{Guid.NewGuid():N}"[..20];
    }
}
