using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using VitaliaBackend.Tenant.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VitaliaBackend.Clinical.Domain.Services;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class DiagnosisCommandService(
    IDiagnosisRepository diagnosisRepository,
    IMedicalRecordRepository medicalRecordRepository,
    IAppointmentRepository appointmentRepository,
    IBranchRepository branchRepository,
    IDiagnosisCatalogProvider diagnosisCatalogProvider,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IDiagnosisCommandService
{
    public async Task<Result<Diagnosis>> Handle(CreateDiagnosisCommand command, CancellationToken cancellationToken)
    {
        if (command.MedicalRecordId == Guid.Empty ||
            !HasValidCie10Code(command.Cie10Code) ||
            !HasValidDescription(command.Description))
            return Result<Diagnosis>.Failure(
                ClinicalError.InvalidDiagnosisDescription,
                localizer[nameof(ClinicalError.InvalidDiagnosisDescription)]);

        var medicalRecord = await medicalRecordRepository.FindByIdAsync(command.MedicalRecordId, cancellationToken);

        if (medicalRecord is null)
            return Result<Diagnosis>.Failure(
                ClinicalError.MedicalRecordNotFound,
                localizer[nameof(ClinicalError.MedicalRecordNotFound)]);

        var catalogItem = await ResolveCatalogItemAsync(
            medicalRecord,
            command.Cie10Code,
            cancellationToken);

        if (catalogItem is null)
            return Result<Diagnosis>.Failure(
                ClinicalError.InvalidDiagnosisCatalogCode,
                localizer[nameof(ClinicalError.InvalidDiagnosisCatalogCode)]);

        var diagnosis = new Diagnosis(
            Guid.NewGuid(),
            GenerateCode(),
            command.MedicalRecordId,
            catalogItem.Code,
            command.Description,
            catalogItem.Source);

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
        UpdateDiagnosisCatalogCommand command,
        CancellationToken cancellationToken)
    {
        if (!HasValidCie10Code(command.Cie10Code) || !HasValidDescription(command.Description))
            return Result<Diagnosis>.Failure(
                ClinicalError.InvalidDiagnosisDescription,
                localizer[nameof(ClinicalError.InvalidDiagnosisDescription)]);

        var diagnosis = await diagnosisRepository.FindByIdAsync(command.DiagnosisId, cancellationToken);

        if (diagnosis is null)
            return Result<Diagnosis>.Failure(
                ClinicalError.DiagnosisNotFound,
                localizer[nameof(ClinicalError.DiagnosisNotFound)]);

        var medicalRecord = await medicalRecordRepository.FindByIdAsync(diagnosis.MedicalRecordId, cancellationToken);

        if (medicalRecord is null)
            return Result<Diagnosis>.Failure(
                ClinicalError.MedicalRecordNotFound,
                localizer[nameof(ClinicalError.MedicalRecordNotFound)]);

        var catalogItem = await ResolveCatalogItemAsync(
            medicalRecord,
            command.Cie10Code,
            cancellationToken);

        if (catalogItem is null)
            return Result<Diagnosis>.Failure(
                ClinicalError.InvalidDiagnosisCatalogCode,
                localizer[nameof(ClinicalError.InvalidDiagnosisCatalogCode)]);

        diagnosis.UpdateCatalogDetails(catalogItem.Code, command.Description, catalogItem.Source);

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

    private static bool HasValidCie10Code(string cie10Code)
    {
        return !string.IsNullOrWhiteSpace(cie10Code);
    }

    private async Task<Application.Model.DiagnosisCatalogItem?> ResolveCatalogItemAsync(
        MedicalRecord medicalRecord,
        string cie10Code,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.FindByIdAsync(medicalRecord.AppointmentId, cancellationToken);

        if (appointment is null)
            return null;

        var branch = await branchRepository.FindByPublicIdAsync(appointment.BranchId, cancellationToken);

        if (branch is null)
            return null;

        return await diagnosisCatalogProvider.FindByCodeAsync(
            branch.DiagnosisCatalogSource,
            cie10Code.Trim(),
            cancellationToken);
    }

    private static string GenerateCode()
    {
        return $"dgn-{Guid.NewGuid():N}"[..20];
    }
}
