using VitaliaBackend.Pharmacy.Application.CommandServices;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Pharmacy.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Pharmacy.Application.Internal.CommandServices;

public class MedicineCommandService(IMedicineRepository medicineRepository, IUnitOfWork unitOfWork)
    : IMedicineCommandService
{
    public async Task<Medicine?> Handle(CreateMedicineCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.Name, command.UnitQuantity, command.UnitType, command.Price, command.Stock))
            return null;

        var existsDuplicate = await medicineRepository.ExistsByNameAndPresentationAsync(
            command.Name,
            command.UnitQuantity,
            command.UnitType,
            cancellationToken: cancellationToken);

        if (existsDuplicate)
            return null;

        var medicine = new Medicine(
            command.Name,
            command.UnitQuantity,
            command.UnitType,
            command.Price,
            command.Stock);

        await medicineRepository.AddAsync(medicine, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return medicine;
    }

    public async Task<Medicine?> Handle(UpdateMedicineCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.Name, command.UnitQuantity, command.UnitType, command.Price, command.Stock))
            return null;

        var medicine = await medicineRepository.FindByIdAsync(command.MedicineId, cancellationToken);

        if (medicine is null)
            return null;

        var existsDuplicate = await medicineRepository.ExistsByNameAndPresentationAsync(
            command.Name,
            command.UnitQuantity,
            command.UnitType,
            command.MedicineId,
            cancellationToken);

        if (existsDuplicate)
            return null;

        medicine.UpdateDetails(
            command.Name,
            command.UnitQuantity,
            command.UnitType,
            command.Price,
            command.Stock);

        medicineRepository.Update(medicine);
        await unitOfWork.CompleteAsync(cancellationToken);

        return medicine;
    }

    public async Task<bool> Handle(DeleteMedicineCommand command, CancellationToken cancellationToken)
    {
        var medicine = await medicineRepository.FindByIdAsync(command.MedicineId, cancellationToken);

        if (medicine is null)
            return false;

        medicineRepository.Remove(medicine);
        await unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }

    private static bool IsValid(string name, int unitQuantity, string unitType, decimal price, int stock)
    {
        return !string.IsNullOrWhiteSpace(name)
               && !string.IsNullOrWhiteSpace(unitType)
               && unitQuantity > 0
               && price >= 0
               && stock >= 0;
    }
}
