namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record UpdatePrescriptionDetailResource(
    int Quantity,
    int Frequency,
    int Duration
);
