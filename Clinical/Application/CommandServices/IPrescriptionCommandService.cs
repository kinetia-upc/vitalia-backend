using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IPrescriptionCommandService
{
    Task<Prescription?> Handle(CreatePrescriptionCommand command, CancellationToken cancellationToken);
}
