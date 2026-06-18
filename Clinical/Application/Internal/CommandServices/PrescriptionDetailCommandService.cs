using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Model.ValueObjects;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class PrescriptionDetailCommandService(
    IPrescriptionDetailRepository prescriptionDetailRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IPrescriptionDetailCommandService
{
    public async Task<Result<PrescriptionDetail>> Handle(
        CreatePrescriptionDetailCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsValid(command))
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        if (!Enum.TryParse<DoseUnitType>(command.DoseUnit, true, out var doseUnit))
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var prescriptionDetail = new PrescriptionDetail(
            command.PrescriptionId,
            command.MedicineId,
            command.MedicineName?.Trim(),
            new Dose(command.DoseAmount, doseUnit),
            command.Frequency.Trim(),
            command.Duration.Trim());

        try
        {
            await prescriptionDetailRepository.AddAsync(prescriptionDetail, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result<PrescriptionDetail>.Success(prescriptionDetail);
        }
        catch (OperationCanceledException)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.OperationCancelled,
                localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.DatabaseError,
                localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InternalServerError,
                localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    private static bool IsValid(CreatePrescriptionDetailCommand command)
    {
        return command.PrescriptionId > 0
               && (command.MedicineId.HasValue || !string.IsNullOrWhiteSpace(command.MedicineName))
               && command.DoseAmount > 0
               && !string.IsNullOrWhiteSpace(command.DoseUnit)
               && !string.IsNullOrWhiteSpace(command.Frequency)
               && command.Frequency.Trim().Length <= 40
               && !string.IsNullOrWhiteSpace(command.Duration)
               && command.Duration.Trim().Length <= 40;
    }
}
