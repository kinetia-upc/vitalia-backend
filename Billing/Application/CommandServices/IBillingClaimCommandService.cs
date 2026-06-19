using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Domain.Model.Commands;
using VitaliaBackend.Shared.Application.Model;

namespace VitaliaBackend.Billing.Application.CommandServices;

/// <summary>
///     Declares every change (create, update, delete) that the Billing bounded
///     context can perform on a billing claim. The interface layer depends only
///     on this contract, never on the concrete implementation.
/// </summary>
public interface IBillingClaimCommandService
{
    Task<Result<BillingClaim>> Handle(CreateBillingClaimCommand command, CancellationToken cancellationToken);
    Task<Result<BillingClaim>> Handle(UpdateBillingClaimCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteBillingClaimCommand command, CancellationToken cancellationToken);
}
