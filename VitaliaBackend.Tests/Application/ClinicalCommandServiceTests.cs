using Microsoft.Extensions.Localization;
using Moq;
using VitaliaBackend.Clinical.Application.Internal.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Tests.Application;

public class ClinicalCommandServiceTests
{
    [Fact]
    public async Task MedicalRecordCommandService_Create_ReusesNumberFreedByDeletion()
    {
        // Simulates: rec-00001..rec-00008 existed, rec-00008 got deleted, so only
        // rec-00001..rec-00007 remain. The next generated code must be rec-00008 again,
        // not rec-00009 - it must be based on the current max in the table, not a fixed count.
        var appointmentId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var appointment = new Appointment(
            appointmentId, "apt-00099", Guid.NewGuid(), patientId, "branch-1",
            DateTime.UtcNow, "Follow-up", EAppointmentStatus.Scheduled);

        MedicalRecord? persistedMedicalRecord = null;

        var medicalRecordRepository = new Mock<IMedicalRecordRepository>();
        medicalRecordRepository
            .Setup(repository => repository.ExistsByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        medicalRecordRepository
            .Setup(repository => repository.GetMaxCodeNumberAsync("rec-", It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);
        medicalRecordRepository
            .Setup(repository => repository.ExistsByCodeAsync("rec-00008", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        medicalRecordRepository
            .Setup(repository => repository.AddAsync(It.IsAny<MedicalRecord>(), It.IsAny<CancellationToken>()))
            .Callback<MedicalRecord, CancellationToken>((medicalRecord, _) => persistedMedicalRecord = medicalRecord)
            .Returns(Task.CompletedTask);

        var appointmentRepository = new Mock<IAppointmentRepository>();
        appointmentRepository
            .Setup(repository => repository.FindByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(work => work.CompleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var localizer = CreateLocalizer();
        var service = new MedicalRecordCommandService(
            medicalRecordRepository.Object, appointmentRepository.Object, unitOfWork.Object, localizer.Object);

        var result = await service.Handle(new CreateClinicalRecordCommand(appointmentId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(persistedMedicalRecord);
        Assert.Equal("rec-00008", persistedMedicalRecord!.Code);
    }

    [Fact]
    public void CreateDiagnosisCommandFromResourceAssembler_MapsWithoutCode()
    {
        var medicalRecordId = Guid.NewGuid();
        var resource = new CreateDiagnosisResource(medicalRecordId, "Primary hypertension");

        var command = CreateDiagnosisCommandFromResourceAssembler.ToCommandFromResource(resource);

        Assert.Equal(medicalRecordId, command.MedicalRecordId);
        Assert.Equal("Primary hypertension", command.Description);
    }

    [Fact]
    public async Task DiagnosisCommandService_Create_GeneratesInternalCode()
    {
        var medicalRecordId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var medicalRecord = new MedicalRecord(Guid.NewGuid(), "rec-00001", Guid.NewGuid(), patientId);
        Diagnosis? persistedDiagnosis = null;

        var diagnosisRepository = new Mock<IDiagnosisRepository>();
        diagnosisRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Diagnosis>(), It.IsAny<CancellationToken>()))
            .Callback<Diagnosis, CancellationToken>((diagnosis, _) => persistedDiagnosis = diagnosis)
            .Returns(Task.CompletedTask);

        var medicalRecordRepository = new Mock<IMedicalRecordRepository>();
        medicalRecordRepository
            .Setup(repository => repository.FindByIdAsync(medicalRecordId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(medicalRecord);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(work => work.CompleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var localizer = CreateLocalizer();
        var service = new DiagnosisCommandService(diagnosisRepository.Object, medicalRecordRepository.Object, unitOfWork.Object, localizer.Object);

        var result = await service.Handle(new CreateDiagnosisCommand(medicalRecordId, "Primary hypertension"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(persistedDiagnosis);
        Assert.Equal(medicalRecordId, persistedDiagnosis!.MedicalRecordId);
        Assert.StartsWith("dgn-", persistedDiagnosis.Code);
        Assert.Equal("Primary hypertension", persistedDiagnosis.Description);
    }

    [Fact]
    public async Task TreatmentCommandService_Create_GeneratesInternalCode()
    {
        var medicalRecordId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var medicalRecord = new MedicalRecord(Guid.NewGuid(), "rec-00001", Guid.NewGuid(), patientId);
        Treatment? persistedTreatment = null;

        var treatmentRepository = new Mock<ITreatmentRepository>();
        treatmentRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Treatment>(), It.IsAny<CancellationToken>()))
            .Callback<Treatment, CancellationToken>((treatment, _) => persistedTreatment = treatment)
            .Returns(Task.CompletedTask);

        var medicalRecordRepository = new Mock<IMedicalRecordRepository>();
        medicalRecordRepository
            .Setup(repository => repository.FindByIdAsync(medicalRecordId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(medicalRecord);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(work => work.CompleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var localizer = CreateLocalizer();
        var service = new TreatmentCommandService(treatmentRepository.Object, medicalRecordRepository.Object, unitOfWork.Object, localizer.Object);

        var result = await service.Handle(new CreateTreatmentCommand(medicalRecordId, "Hydration and rest"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(persistedTreatment);
        Assert.Equal(medicalRecordId, persistedTreatment!.MedicalRecordId);
        Assert.StartsWith("trt-", persistedTreatment.Code);
        Assert.Equal("Hydration and rest", persistedTreatment.Description);
    }

    private static Mock<IStringLocalizer<ErrorMessages>> CreateLocalizer()
    {
        var localizer = new Mock<IStringLocalizer<ErrorMessages>>();
        localizer
            .Setup(instance => instance[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));
        return localizer;
    }
}
