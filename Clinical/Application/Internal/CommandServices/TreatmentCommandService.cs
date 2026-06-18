using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class TreatmentCommandService(ITreatmentRepository treatmentRepository, IUnitOfWork unitOfWork)
    : ITreatmentCommandService
{
    public async Task<Treatment?> Handle(CreateTreatmentCommand command, CancellationToken cancellationToken)
    {
        if (!HasValidDescription(command.Description))
            return null;

        var treatment = new Treatment(command.Description.Trim());

        await treatmentRepository.AddAsync(treatment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return treatment;
    }

    private static bool HasValidDescription(string description)
    {
        return !string.IsNullOrWhiteSpace(description) && description.Trim().Length >= 5;
    }
}
