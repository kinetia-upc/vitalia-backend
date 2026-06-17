using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class CreateMedicalRecordCommandFromResourceAssembler
{
    public static CreateClinicalRecordCommand ToCommandFromResource(CreateMedicalRecordResource resource)
    {
        return new CreateClinicalRecordCommand(
            resource.PatientId,
            resource.AppointmentId);
    }
}
