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

public class DiagnosisCommandService(
    IDiagnosisRepository diagnosisRepository,
    IMedicalRecordRepository medicalRecordRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IDiagnosisCommandService
{
    public async Task<Result<Diagnosis>> Handle(CreateDiagnosisCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.MedicalRecordId) || !HasValidDescription(command.Description))
            return Result<Diagnosis>.Failure(
                ClinicalError.InvalidDiagnosisDescription,
                localizer[nameof(ClinicalError.InvalidDiagnosisDescription)]);

        var medicalRecordExists = await medicalRecordRepository.ExistsByCodeAsync(
            command.MedicalRecordId,
            cancellationToken);

        if (!medicalRecordExists)
            return Result<Diagnosis>.Failure(
                ClinicalError.MedicalRecordNotFound,
                localizer[nameof(ClinicalError.MedicalRecordNotFound)]);

        var diagnosis = new Diagnosis(command.MedicalRecordId, command.Description);

        try
        {
            await diagnosisRepository.AddAsync(diagnosis, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Diagnosis>.Success(diagnosis);
        }
        catch (OperationCanceledException)
        {
            return Result<Diagnosis>.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Diagnosis>.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Diagnosis>.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    public async Task<Result<Diagnosis>> Handle(
        UpdateDiagnosisDescriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (!HasValidDescription(command.Description))
            return Result<Diagnosis>.Failure(
                ClinicalError.InvalidDiagnosisDescription,
                localizer[nameof(ClinicalError.InvalidDiagnosisDescription)]);

        var diagnosis = await diagnosisRepository.FindByIdAsync(command.DiagnosisId, cancellationToken);

        if (diagnosis is null)
            return Result<Diagnosis>.Failure(
                ClinicalError.DiagnosisNotFound,
                localizer[nameof(ClinicalError.DiagnosisNotFound)]);

        diagnosis.UpdateDescription(command.Description);

        try
        {
            diagnosisRepository.Update(diagnosis);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Diagnosis>.Success(diagnosis);
        }
        catch (OperationCanceledException)
        {
            return Result<Diagnosis>.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Diagnosis>.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Diagnosis>.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteDiagnosisCommand command, CancellationToken cancellationToken)
    {
        var diagnosis = await diagnosisRepository.FindByIdAsync(command.DiagnosisId, cancellationToken);

        if (diagnosis is null)
            return Result.Failure(
                ClinicalError.DiagnosisNotFound,
                localizer[nameof(ClinicalError.DiagnosisNotFound)]);

        try
        {
            diagnosisRepository.Remove(diagnosis);
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
}
