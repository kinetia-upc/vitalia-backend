using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Model.Commands;

namespace VitaliaBackend.Pharmacy.Application.CommandServices;

public interface IMedicineCommandService
{
    Task<Medicine?> Handle(CreateMedicineCommand command, CancellationToken cancellationToken);
    Task<Medicine?> Handle(UpdateMedicineCommand command, CancellationToken cancellationToken);
    Task<bool> Handle(DeleteMedicineCommand command, CancellationToken cancellationToken);
}
