using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

public record CreateBranchResource(
    [property: SwaggerSchema(Description = "Business id of the branch, e.g. 'branch-001'. Client-supplied, must be unique.")]
    string Id,

    [property: SwaggerSchema(Description = "Business id of the healthcare center this branch belongs to.")]
    string HealthcareCenterId,

    [property: SwaggerSchema(Description = "Business id of the address record for this branch, if registered.")]
    string? AddressId,

    [property: SwaggerSchema(Description = "Display name of the branch, e.g. 'Sede San Borja'.")]
    string BranchName,

    [property: SwaggerSchema(Description = "Full street address of the branch.")]
    string Address
);
