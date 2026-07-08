using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Pharmacy.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class PrescriptionDetailCommandService(
    IPrescriptionDetailRepository prescriptionDetailRepository,
    IPrescriptionRepository prescriptionRepository,
    IMedicalRecordRepository medicalRecordRepository,
    IAppointmentRepository appointmentRepository,
    IMedicineRepository medicineRepository,
    IBranchMedicineRepository branchMedicineRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IPrescriptionDetailCommandService
{
    public async Task<Result<PrescriptionDetail>> Handle(
        CreatePrescriptionDetailCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsValid(command))
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var prescription = await prescriptionRepository.FindByIdAsync(command.PrescriptionId, cancellationToken);

        if (prescription is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.PrescriptionNotFound,
                localizer[nameof(ClinicalError.PrescriptionNotFound)]);

        var medicine = await medicineRepository.FindByIdAsync(command.MedicineId, cancellationToken);
        if (medicine is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var medicalRecord = await medicalRecordRepository.FindByIdAsync(prescription.MedicalRecordId, cancellationToken);
        if (medicalRecord is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var appointment = await appointmentRepository.FindByIdAsync(medicalRecord.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var branchMedicine = await branchMedicineRepository.FindByBranchAndMedicineAsync(
            appointment.BranchId, command.MedicineId, cancellationToken);
        if (branchMedicine is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        if (!branchMedicine.HasEnoughStock(command.Quantity))
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        branchMedicine.TryDecreaseStock(command.Quantity);

        var prescriptionDetail = new PrescriptionDetail(
            command.PrescriptionId,
            command.MedicineId,
            command.Quantity,
            command.Frequency,
            command.Duration);

        try
        {
            await prescriptionDetailRepository.AddAsync(prescriptionDetail, cancellationToken);
            branchMedicineRepository.Update(branchMedicine);
            medicalRecordRepository.Update(medicalRecord);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result<PrescriptionDetail>.Success(prescriptionDetail);
        }
        catch (OperationCanceledException)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.OperationCancelled,
                localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.DatabaseError,
                localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InternalServerError,
                localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    public async Task<Result<PrescriptionDetail>> Handle(
        UpdatePrescriptionDetailCommand command,
        CancellationToken cancellationToken)
    {
        if (!IsValid(command))
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var medicine = await medicineRepository.FindByIdAsync(command.MedicineId, cancellationToken);
        if (medicine is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InvalidPrescriptionDetailData,
                localizer[nameof(ClinicalError.InvalidPrescriptionDetailData)]);

        var prescriptionDetail = await prescriptionDetailRepository.FindByPrescriptionAndMedicineAsync(
            command.PrescriptionId,
            command.MedicineId,
            cancellationToken);

        if (prescriptionDetail is null)
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.PrescriptionDetailNotFound,
                localizer[nameof(ClinicalError.PrescriptionDetailNotFound)]);

        prescriptionDetail.UpdateDetails(
            command.Quantity,
            command.Frequency,
            command.Duration);

        var medicalRecord = await FindMedicalRecordByPrescriptionIdAsync(command.PrescriptionId, cancellationToken);

        try
        {
            prescriptionDetailRepository.Update(prescriptionDetail);
            if (medicalRecord is not null) medicalRecordRepository.Update(medicalRecord);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result<PrescriptionDetail>.Success(prescriptionDetail);
        }
        catch (OperationCanceledException)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.OperationCancelled,
                localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.DatabaseError,
                localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<PrescriptionDetail>.Failure(
                ClinicalError.InternalServerError,
                localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeletePrescriptionDetailCommand command, CancellationToken cancellationToken)
    {
        var prescriptionDetail = await prescriptionDetailRepository.FindByPrescriptionAndMedicineAsync(
            command.PrescriptionId,
            command.MedicineId,
            cancellationToken);

        if (prescriptionDetail is null)
            return Result.Failure(
                ClinicalError.PrescriptionDetailNotFound,
                localizer[nameof(ClinicalError.PrescriptionDetailNotFound)]);

        var medicalRecord = await FindMedicalRecordByPrescriptionIdAsync(command.PrescriptionId, cancellationToken);

        try
        {
            prescriptionDetailRepository.Remove(prescriptionDetail);
            if (medicalRecord is not null) medicalRecordRepository.Update(medicalRecord);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(ClinicalError.OperationCancelled, localizer[nameof(ClinicalError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(ClinicalError.DatabaseError, localizer[nameof(ClinicalError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result.Failure(ClinicalError.InternalServerError, localizer[nameof(ClinicalError.InternalServerError)]);
        }
    }

    private static bool IsValid(CreatePrescriptionDetailCommand command)
    {
        return command.PrescriptionId != Guid.Empty
               && command.MedicineId != Guid.Empty
               && command.Quantity > 0
               && command.Frequency > 0
               && command.Duration > 0;
    }

    private static bool IsValid(UpdatePrescriptionDetailCommand command)
    {
        return command.PrescriptionId != Guid.Empty
               && command.MedicineId != Guid.Empty
               && command.Quantity > 0
               && command.Frequency > 0
               && command.Duration > 0;
    }

    private async Task<MedicalRecord?> FindMedicalRecordByPrescriptionIdAsync(
        Guid prescriptionId,
        CancellationToken cancellationToken)
    {
        var prescription = await prescriptionRepository.FindByIdAsync(prescriptionId, cancellationToken);
        if (prescription is null) return null;

        return await medicalRecordRepository.FindByIdAsync(prescription.MedicalRecordId, cancellationToken);
    }
}
