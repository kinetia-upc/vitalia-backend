using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

public record UpdateBranchResource(
    [property: SwaggerSchema(Description = "Business id of the healthcare center this branch belongs to.")]
    string HealthcareCenterId,

    [property: SwaggerSchema(Description = "Display name of the branch, e.g. 'Sede San Borja'.")]
    string BranchName,

    [property: SwaggerSchema(Description = "Full street address of the branch.")]
    string Address
);
