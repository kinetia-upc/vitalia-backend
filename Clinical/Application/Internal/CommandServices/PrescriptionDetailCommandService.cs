using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Model.ValueObjects;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class PrescriptionDetailCommandService(
    IPrescriptionDetailRepository prescriptionDetailRepository,
    IUnitOfWork unitOfWork)
    : IPrescriptionDetailCommandService
{
    public async Task<PrescriptionDetail?> Handle(
        CreatePrescriptionDetailCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsValid(command))
            return null;

        if (!Enum.TryParse<DoseUnitType>(command.DoseUnit, true, out var doseUnit))
            return null;

        var prescriptionDetail = new PrescriptionDetail(
            command.PrescriptionId,
            command.MedicineId,
            command.MedicineName?.Trim(),
            new Dose(command.DoseAmount, doseUnit),
            command.Frequency.Trim(),
            command.Duration.Trim());

        await prescriptionDetailRepository.AddAsync(prescriptionDetail, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return prescriptionDetail;
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
