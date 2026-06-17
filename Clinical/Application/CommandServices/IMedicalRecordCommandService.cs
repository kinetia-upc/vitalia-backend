using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IMedicalRecordCommandService
{
    Task<MedicalRecord?> Handle(CreateClinicalRecordCommand command, CancellationToken cancellationToken);
}
