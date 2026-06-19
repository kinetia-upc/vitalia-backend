namespace VitaliaBackend.Billing.Domain.Model;

public enum BillingError
{
    None,
    BillingClaimCreationError,
    BillingClaimUpdateError,
    BillingClaimDeletionError,
    BillingClaimNotFoundError,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
