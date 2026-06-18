using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class PrescriptionCommandService(IPrescriptionRepository prescriptionRepository, IUnitOfWork unitOfWork)
    : IPrescriptionCommandService
{
    public async Task<Prescription?> Handle(CreatePrescriptionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.MedicalRecordId))
            return null;

        var prescription = new Prescription(command.MedicalRecordId.Trim());

        await prescriptionRepository.AddAsync(prescription, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return prescription;
    }
}
