using VitaliaBackend.Billing.Application.CommandServices;
using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Model.Commands;
using VitaliaBackend.Billing.Domain.Model.ValueObjects;
using VitaliaBackend.Billing.Domain.Repositories;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Billing.Application.Internal.CommandServices;

/// <summary>
///     Implements every write use case for billing claims: validate the incoming data,
///     apply the change to the aggregate, persist it through the repository and confirm
///     the change through the unit of work.
/// </summary>
public class BillingClaimCommandService(IBillingClaimRepository billingClaimRepository, IUnitOfWork unitOfWork)
    : IBillingClaimCommandService
{
    public async Task<BillingClaim?> Handle(CreateBillingClaimCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(
                command.ClaimCode,
                command.InsuranceProvider,
                command.PatientName,
                command.ProviderName,
                command.Value,
                command.ClinicalCompliance,
                command.CycleStatus))
            return null;

        var existsDuplicate = await billingClaimRepository.ExistsByClaimCodeAsync(
            command.ClaimCode,
            cancellationToken: cancellationToken);

        if (existsDuplicate)
            return null;

        var billingClaim = new BillingClaim(
            command.ClaimCode,
            command.InsuranceProvider,
            command.PatientName,
            command.ProviderName,
            command.Value,
            command.ClinicalCompliance,
            command.CycleStatus);

        await billingClaimRepository.AddAsync(billingClaim, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return billingClaim;
    }

    public async Task<BillingClaim?> Handle(UpdateBillingClaimCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(
                command.ClaimCode,
                command.InsuranceProvider,
                command.PatientName,
                command.ProviderName,
                command.Value,
                command.ClinicalCompliance,
                command.CycleStatus))
            return null;

        var billingClaim = await billingClaimRepository.FindByIdAsync(command.BillingClaimId, cancellationToken);

        if (billingClaim is null)
            return null;

        var existsDuplicate = await billingClaimRepository.ExistsByClaimCodeAsync(
            command.ClaimCode,
            command.BillingClaimId,
            cancellationToken);

        if (existsDuplicate)
            return null;

        billingClaim.UpdateDetails(
            command.ClaimCode,
            command.InsuranceProvider,
            command.PatientName,
            command.ProviderName,
            command.Value,
            command.ClinicalCompliance,
            command.CycleStatus);

        billingClaimRepository.Update(billingClaim);
        await unitOfWork.CompleteAsync(cancellationToken);

        return billingClaim;
    }

    public async Task<bool> Handle(DeleteBillingClaimCommand command, CancellationToken cancellationToken)
    {
        var billingClaim = await billingClaimRepository.FindByIdAsync(command.BillingClaimId, cancellationToken);

        if (billingClaim is null)
            return false;

        billingClaimRepository.Remove(billingClaim);
        await unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }

    /// <summary>
    ///     Checks that every field has a value that makes sense for a billing claim,
    ///     including the two status fields, which must match one of the allowed values
    ///     defined in <see cref="BillingClaimStatuses" />.
    /// </summary>
    private static bool IsValid(
        string claimCode,
        string insuranceProvider,
        string patientName,
        string providerName,
        decimal value,
        string clinicalCompliance,
        string cycleStatus)
    {
        return !string.IsNullOrWhiteSpace(claimCode)
               && !string.IsNullOrWhiteSpace(insuranceProvider)
               && !string.IsNullOrWhiteSpace(patientName)
               && !string.IsNullOrWhiteSpace(providerName)
               && value >= 0
               && BillingClaimStatuses.AllowedClinicalCompliances.Contains(clinicalCompliance)
               && BillingClaimStatuses.AllowedCycleStatuses.Contains(cycleStatus);
    }
}
