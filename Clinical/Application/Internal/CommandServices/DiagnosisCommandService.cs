using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class DiagnosisCommandService(IDiagnosisRepository diagnosisRepository, IUnitOfWork unitOfWork)
    : IDiagnosisCommandService
{
    public async Task<Diagnosis?> Handle(CreateDiagnosisCommand command, CancellationToken cancellationToken)
    {
        if (!HasValidDescription(command.Description))
            return null;

        var diagnosis = new Diagnosis(command.Description.Trim());

        await diagnosisRepository.AddAsync(diagnosis, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return diagnosis;
    }

    private static bool HasValidDescription(string description)
    {
        return !string.IsNullOrWhiteSpace(description) && description.Trim().Length >= 5;
    }
}
