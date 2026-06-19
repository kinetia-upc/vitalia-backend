namespace VitaliaBackend.Clinical.Domain.Model.ValueObjects;

public record Dose(int Amount, DoseUnitType Unit)
{
    public Dose() : this(0, DoseUnitType.Mg)
    {
    }

    public Dose(int amount) : this(amount, DoseUnitType.Mg)
    {
    }

    public string FullDose => $"{Amount} {Unit}";
}