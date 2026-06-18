using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IDiagnosisCommandService
{
    Task<Result<Diagnosis>> Handle(CreateDiagnosisCommand command, CancellationToken cancellationToken);
    Task<Result<Diagnosis>> Handle(UpdateDiagnosisDescriptionCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteDiagnosisCommand command, CancellationToken cancellationToken);
}
