using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IMedicalRecordCommandService
{
    Task<Result<MedicalRecord>> Handle(CreateClinicalRecordCommand command, CancellationToken cancellationToken);
}
