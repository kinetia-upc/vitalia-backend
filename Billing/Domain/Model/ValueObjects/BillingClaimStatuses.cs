using VitaliaBackend.Billing.Domain.Model.Aggregates;

namespace VitaliaBackend.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Holds the closed list of valid string values for the status fields of a billing claim.
/// </summary>
/// <remarks>
///     A C# enum was not used here because the values exposed by the frontend contain spaces
///     ("In Clearinghouse") and underscores ("missing_icd10"), which are not valid C# enum
///     member names. Plain strings, validated against this fixed list, keep the backend in
///     sync with the frontend without needing extra mapping code.
/// </remarks>
public static class BillingClaimStatuses
{
    /// <summary>
    ///     Valid values for <see cref="BillingClaim.ClinicalCompliance" />.
    /// </summary>
    public static readonly string[] AllowedClinicalCompliances =
    [
        "verified",
        "pending",
        "pending_sign",
        "missing_icd10"
    ];

    /// <summary>
    ///     Valid values for <see cref="BillingClaim.CycleStatus" />.
    /// </summary>
    public static readonly string[] AllowedCycleStatuses =
    [
        "In Clearinghouse",
        "Funds Released",
        "Auth Required",
        "Rejected",
        "cleared",
        "submitted"
    ];
}
