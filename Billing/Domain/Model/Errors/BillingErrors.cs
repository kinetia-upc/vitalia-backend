using VitaliaBackend.Shared.Domain.Model;

namespace VitaliaBackend.Billing.Domain.Model.Errors;

/// <summary>
///     Fixed catalog of error codes and messages used by the Billing bounded context,
///     so every controller returns the same error for the same problem.
/// </summary>
public static class BillingErrors
{
    public static readonly Error BillingClaimCreationError =
        new("Billing.BillingClaimCreationError", "Error creating billing claim");

    public static readonly Error BillingClaimUpdateError =
        new("Billing.BillingClaimUpdateError", "Error updating billing claim");

    public static readonly Error BillingClaimDeletionError =
        new("Billing.BillingClaimDeletionError", "Error deleting billing claim");

    public static readonly Error BillingClaimNotFoundError =
        new("Billing.BillingClaimNotFoundError", "Billing claim not found");
}
