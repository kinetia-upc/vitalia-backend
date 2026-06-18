using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IPrescriptionDetailCommandService
{
    Task<PrescriptionDetail?> Handle(CreatePrescriptionDetailCommand command, CancellationToken cancellationToken);
}
