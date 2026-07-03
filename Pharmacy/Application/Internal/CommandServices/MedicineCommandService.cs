using VitaliaBackend.Pharmacy.Application.CommandServices;
using VitaliaBackend.Pharmacy.Domain.Model;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Pharmacy.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Pharmacy.Application.Internal.CommandServices;

public class MedicineCommandService(
    IMedicineRepository medicineRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IMedicineCommandService
{
    public async Task<Result<Medicine>> Handle(CreateMedicineCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.Code, command.Name, command.UnitQuantity, command.UnitType))
            return Result<Medicine>.Failure(
                PharmacyError.MedicineCreationError,
                localizer[nameof(PharmacyError.MedicineCreationError)]);

        if (await medicineRepository.ExistsByCodeAsync(command.Code, cancellationToken: cancellationToken))
            return Result<Medicine>.Failure(
                PharmacyError.MedicineCreationError,
                localizer[nameof(PharmacyError.MedicineCreationError)]);

        var existsDuplicate = await medicineRepository.ExistsByNameAndPresentationAsync(
            command.Name,
            command.UnitQuantity,
            command.UnitType,
            cancellationToken: cancellationToken);

        if (existsDuplicate)
            return Result<Medicine>.Failure(
                PharmacyError.MedicineCreationError,
                localizer[nameof(PharmacyError.MedicineCreationError)]);

        var medicine = new Medicine(
            Guid.NewGuid(),
            command.Code,
            command.Name,
            command.UnitQuantity,
            command.UnitType);

        try
        {
            await medicineRepository.AddAsync(medicine, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Medicine>.Success(medicine);
        }
        catch (OperationCanceledException)
        {
            return Result<Medicine>.Failure(PharmacyError.OperationCancelled, localizer[nameof(PharmacyError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Medicine>.Failure(PharmacyError.DatabaseError, localizer[nameof(PharmacyError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Medicine>.Failure(PharmacyError.InternalServerError, localizer[nameof(PharmacyError.InternalServerError)]);
        }
    }

    public async Task<Result<Medicine>> Handle(UpdateMedicineCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.Code, command.Name, command.UnitQuantity, command.UnitType))
            return Result<Medicine>.Failure(
                PharmacyError.MedicineUpdateError,
                localizer[nameof(PharmacyError.MedicineUpdateError)]);

        var medicine = await medicineRepository.FindByIdAsync(command.MedicineId, cancellationToken);

        if (medicine is null)
            return Result<Medicine>.Failure(
                PharmacyError.MedicineNotFoundError,
                localizer[nameof(PharmacyError.MedicineNotFoundError)]);

        if (await medicineRepository.ExistsByCodeAsync(command.Code, command.MedicineId, cancellationToken))
            return Result<Medicine>.Failure(
                PharmacyError.MedicineUpdateError,
                localizer[nameof(PharmacyError.MedicineUpdateError)]);

        var existsDuplicate = await medicineRepository.ExistsByNameAndPresentationAsync(
            command.Name,
            command.UnitQuantity,
            command.UnitType,
            command.MedicineId,
            cancellationToken);

        if (existsDuplicate)
            return Result<Medicine>.Failure(
                PharmacyError.MedicineUpdateError,
                localizer[nameof(PharmacyError.MedicineUpdateError)]);

        medicine.UpdateDetails(
            command.Code,
            command.Name,
            command.UnitQuantity,
            command.UnitType);

        try
        {
            medicineRepository.Update(medicine);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Medicine>.Success(medicine);
        }
        catch (OperationCanceledException)
        {
            return Result<Medicine>.Failure(PharmacyError.OperationCancelled, localizer[nameof(PharmacyError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Medicine>.Failure(PharmacyError.DatabaseError, localizer[nameof(PharmacyError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Medicine>.Failure(PharmacyError.InternalServerError, localizer[nameof(PharmacyError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteMedicineCommand command, CancellationToken cancellationToken)
    {
        var medicine = await medicineRepository.FindByIdAsync(command.MedicineId, cancellationToken);

        if (medicine is null)
            return Result.Failure(
                PharmacyError.MedicineNotFoundError,
                localizer[nameof(PharmacyError.MedicineNotFoundError)]);

        try
        {
            medicineRepository.Remove(medicine);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(PharmacyError.OperationCancelled, localizer[nameof(PharmacyError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(PharmacyError.DatabaseError, localizer[nameof(PharmacyError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result.Failure(PharmacyError.InternalServerError, localizer[nameof(PharmacyError.InternalServerError)]);
        }
    }

    private static bool IsValid(string code, string name, int unitQuantity, string unitType)
    {
        return !string.IsNullOrWhiteSpace(code)
               && !string.IsNullOrWhiteSpace(name)
               && !string.IsNullOrWhiteSpace(unitType)
               && unitQuantity > 0;
     }
}
