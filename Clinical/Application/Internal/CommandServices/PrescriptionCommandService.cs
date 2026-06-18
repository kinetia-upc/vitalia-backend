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
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IPrescriptionCommandService
{
    public async Task<Result<Prescription>> Handle(CreatePrescriptionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.MedicalRecordId))
            return Result<Prescription>.Failure(
                ClinicalError.InvalidPrescriptionData,
                localizer[nameof(ClinicalError.InvalidPrescriptionData)]);

        var prescription = new Prescription(command.MedicalRecordId.Trim());

        try
        {
            await prescriptionRepository.AddAsync(prescription, cancellationToken);
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
}
