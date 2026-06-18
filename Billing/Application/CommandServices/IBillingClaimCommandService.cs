using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Model.Commands;

namespace VitaliaBackend.Billing.Application.CommandServices;

/// <summary>
///     Declares every change (create, update, delete) that the Billing bounded
///     context can perform on a billing claim. The interface layer depends only
///     on this contract, never on the concrete implementation.
/// </summary>
public interface IBillingClaimCommandService
{
    Task<BillingClaim?> Handle(CreateBillingClaimCommand command, CancellationToken cancellationToken);
    Task<BillingClaim?> Handle(UpdateBillingClaimCommand command, CancellationToken cancellationToken);
    Task<bool> Handle(DeleteBillingClaimCommand command, CancellationToken cancellationToken);
}
