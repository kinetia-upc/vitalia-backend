using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Pharmacy.Application.CommandServices;

public interface IMedicineCommandService
{
    Task<Result<Medicine>> Handle(CreateMedicineCommand command, CancellationToken cancellationToken);
    Task<Result<Medicine>> Handle(UpdateMedicineCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteMedicineCommand command, CancellationToken cancellationToken);
}
