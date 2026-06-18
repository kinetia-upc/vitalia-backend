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
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : ITreatmentCommandService
{
    public async Task<Result<Treatment>> Handle(CreateTreatmentCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.MedicalRecordId) || !HasValidDescription(command.Description))
            return Result<Treatment>.Failure(
                ClinicalError.InvalidTreatmentDescription,
                localizer[nameof(ClinicalError.InvalidTreatmentDescription)]);

        var treatment = new Treatment(command.MedicalRecordId, command.Description);

        try
        {
            await treatmentRepository.AddAsync(treatment, cancellationToken);
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

        try
        {
            treatmentRepository.Update(treatment);
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

    private static bool HasValidDescription(string description)
    {
        return !string.IsNullOrWhiteSpace(description) && description.Trim().Length >= 5;
    }
}
