using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

public record UpdateBranchResource(
    [property: SwaggerSchema(Description = "Business id of the healthcare center this branch belongs to.")]
    string HealthcareCenterId,

    [property: SwaggerSchema(Description = "Display name of the branch, e.g. 'Sede San Borja'.")]
    string BranchName,

    [property: SwaggerSchema(Description = "Full street address of the branch.")]
    string Address,

    [property: SwaggerSchema(Description = "Diagnosis catalog source used by this branch.")]
    DiagnosisCatalogSource DiagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10
);
