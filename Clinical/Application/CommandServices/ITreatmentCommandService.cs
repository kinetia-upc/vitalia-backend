using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface ITreatmentCommandService
{
    Task<Treatment?> Handle(CreateTreatmentCommand command, CancellationToken cancellationToken);
}
