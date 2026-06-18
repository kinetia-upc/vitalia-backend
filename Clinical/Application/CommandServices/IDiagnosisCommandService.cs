using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IDiagnosisCommandService
{
    Task<Diagnosis?> Handle(CreateDiagnosisCommand command, CancellationToken cancellationToken);
}
