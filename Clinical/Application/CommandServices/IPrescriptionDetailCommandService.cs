using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IPrescriptionDetailCommandService
{
    Task<Result<PrescriptionDetail>> Handle(CreatePrescriptionDetailCommand command, CancellationToken cancellationToken);
}
