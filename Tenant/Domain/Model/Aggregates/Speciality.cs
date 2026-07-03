namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

public class Speciality
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }

    protected Speciality()
    {
        Code = string.Empty;
        Description = string.Empty;
    }

    public Speciality(Guid id, string code, string description)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        Description = description.Trim();
    }

    public void UpdateDetails(string description)
    {
        Description = description.Trim();
    }
}
