using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface ITreatmentCommandService
{
    Task<Result<Treatment>> Handle(CreateTreatmentCommand command, CancellationToken cancellationToken);
    Task<Result<Treatment>> Handle(UpdateTreatmentDescriptionCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteTreatmentCommand command, CancellationToken cancellationToken);
}
