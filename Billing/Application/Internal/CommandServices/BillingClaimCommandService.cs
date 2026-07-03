using VitaliaBackend.Billing.Application.CommandServices;
using VitaliaBackend.Billing.Domain.Model;
using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Model.Commands;
using VitaliaBackend.Billing.Domain.Model.ValueObjects;
using VitaliaBackend.Billing.Domain.Repositories;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Billing.Application.Internal.CommandServices;

/// <summary>
///     Implements every write use case for billing claims: validate the incoming data,
///     apply the change to the aggregate, persist it through the repository and confirm
///     the change through the unit of work.
/// </summary>
public class BillingClaimCommandService(
    IBillingClaimRepository billingClaimRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IBillingClaimCommandService
{
    public async Task<Result<BillingClaim>> Handle(CreateBillingClaimCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(
                command.Code,
                command.AppointmentId,
                command.InsuranceProvider,
                command.PatientName,
                command.ProviderName,
                command.Value,
                command.ClinicalCompliance,
                command.CycleStatus))
            return Result<BillingClaim>.Failure(
                BillingError.BillingClaimCreationError,
                localizer[nameof(BillingError.BillingClaimCreationError)]);

        var existsDuplicate = await billingClaimRepository.ExistsByClaimCodeAsync(
            command.Code,
            cancellationToken: cancellationToken);

        if (existsDuplicate)
            return Result<BillingClaim>.Failure(
                BillingError.BillingClaimCreationError,
                localizer[nameof(BillingError.BillingClaimCreationError)]);

        var billingClaim = new BillingClaim(
            command.Code,
            command.AppointmentId,
            command.InsuranceProvider,
            command.PatientName,
            command.ProviderName,
            command.Value,
            command.ClinicalCompliance,
            command.CycleStatus);

        try
        {
            await billingClaimRepository.AddAsync(billingClaim, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<BillingClaim>.Success(billingClaim);
        }
        catch (OperationCanceledException)
        {
            return Result<BillingClaim>.Failure(BillingError.OperationCancelled, localizer[nameof(BillingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<BillingClaim>.Failure(BillingError.DatabaseError, localizer[nameof(BillingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<BillingClaim>.Failure(BillingError.InternalServerError, localizer[nameof(BillingError.InternalServerError)]);
        }
    }

    public async Task<Result<BillingClaim>> Handle(UpdateBillingClaimCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(
                command.Code,
                command.AppointmentId,
                command.InsuranceProvider,
                command.PatientName,
                command.ProviderName,
                command.Value,
                command.ClinicalCompliance,
                command.CycleStatus))
            return Result<BillingClaim>.Failure(
                BillingError.BillingClaimUpdateError,
                localizer[nameof(BillingError.BillingClaimUpdateError)]);

        var billingClaim = await billingClaimRepository.FindByIdAsync(command.BillingClaimId, cancellationToken);

        if (billingClaim is null)
            return Result<BillingClaim>.Failure(
                BillingError.BillingClaimNotFoundError,
                localizer[nameof(BillingError.BillingClaimNotFoundError)]);

        var existsDuplicate = await billingClaimRepository.ExistsByClaimCodeAsync(
            command.Code,
            command.BillingClaimId,
            cancellationToken);

        if (existsDuplicate)
            return Result<BillingClaim>.Failure(
                BillingError.BillingClaimUpdateError,
                localizer[nameof(BillingError.BillingClaimUpdateError)]);

        billingClaim.UpdateDetails(
            command.Code,
            command.AppointmentId,
            command.InsuranceProvider,
            command.PatientName,
            command.ProviderName,
            command.Value,
            command.ClinicalCompliance,
            command.CycleStatus);

        try
        {
            billingClaimRepository.Update(billingClaim);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<BillingClaim>.Success(billingClaim);
        }
        catch (OperationCanceledException)
        {
            return Result<BillingClaim>.Failure(BillingError.OperationCancelled, localizer[nameof(BillingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<BillingClaim>.Failure(BillingError.DatabaseError, localizer[nameof(BillingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<BillingClaim>.Failure(BillingError.InternalServerError, localizer[nameof(BillingError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteBillingClaimCommand command, CancellationToken cancellationToken)
    {
        var billingClaim = await billingClaimRepository.FindByIdAsync(command.BillingClaimId, cancellationToken);

        if (billingClaim is null)
            return Result.Failure(
                BillingError.BillingClaimNotFoundError,
                localizer[nameof(BillingError.BillingClaimNotFoundError)]);

        try
        {
            billingClaimRepository.Remove(billingClaim);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(BillingError.OperationCancelled, localizer[nameof(BillingError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(BillingError.DatabaseError, localizer[nameof(BillingError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result.Failure(BillingError.InternalServerError, localizer[nameof(BillingError.InternalServerError)]);
        }
    }

    /// <summary>
    ///     Checks that every field has a value that makes sense for a billing claim,
    ///     including the two status fields, which must match one of the allowed values
    ///     defined in <see cref="BillingClaimStatuses" />.
    /// </summary>
    private static bool IsValid(
        string claimCode,
        Guid appointmentId,
        string insuranceProvider,
        string patientName,
        string providerName,
        decimal value,
        string clinicalCompliance,
        string cycleStatus)
    {
        return !string.IsNullOrWhiteSpace(claimCode)
               && appointmentId != Guid.Empty
               && !string.IsNullOrWhiteSpace(insuranceProvider)
               && !string.IsNullOrWhiteSpace(patientName)
               && !string.IsNullOrWhiteSpace(providerName)
               && value >= 0
               && BillingClaimStatuses.AllowedClinicalCompliances.Contains(clinicalCompliance)
               && BillingClaimStatuses.AllowedCycleStatuses.Contains(cycleStatus);
    }
}
