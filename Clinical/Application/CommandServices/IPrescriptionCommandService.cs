using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IPrescriptionCommandService
{
    Task<Result<Prescription>> Handle(CreatePrescriptionCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeletePrescriptionCommand command, CancellationToken cancellationToken);
}
