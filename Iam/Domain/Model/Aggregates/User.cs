using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Iam.Domain.Model.Aggregates;

public class User : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string HealthcareCenterId { get; private set; }
    public string Name { get; private set; }
    public string PaternalSurname { get; private set; }
    public string MaternalSurname { get; private set; }
    public string IdentityType { get; private set; }
    public string IdentityNumber { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Phone { get; private set; }
    public string Gender { get; private set; }
    public bool IsActive { get; private set; }
    public string Address { get; private set; }
    public string Role { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected User()
    {
        HealthcareCenterId = string.Empty;
        Name = string.Empty;
        PaternalSurname = string.Empty;
        MaternalSurname = string.Empty;
        IdentityType = string.Empty;
        IdentityNumber = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        Phone = string.Empty;
        Gender = string.Empty;
        Address = string.Empty;
        Role = string.Empty;
    }

    public User(
        Guid id,
        string healthcareCenterId,
        string name,
        string paternalSurname,
        string maternalSurname,
        string identityType,
        string identityNumber,
        DateOnly birthDate,
        string email,
        string passwordHash,
        string phone,
        string gender,
        bool isActive,
        string address,
        string role)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        HealthcareCenterId = healthcareCenterId.Trim();
        Name = name.Trim();
        PaternalSurname = paternalSurname.Trim();
        MaternalSurname = maternalSurname.Trim();
        IdentityType = identityType.Trim();
        IdentityNumber = identityNumber.Trim();
        BirthDate = birthDate;
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Phone = phone.Trim();
        Gender = gender.Trim();
        IsActive = isActive;
        Address = address.Trim();
        Role = role.Trim();
    }

    public void UpdateDetails(
        string healthcareCenterId,
        string name,
        string paternalSurname,
        string maternalSurname,
        string identityType,
        string identityNumber,
        DateOnly birthDate,
        string email,
        string phone,
        string gender,
        bool isActive,
        string address,
        string role)
    {
        HealthcareCenterId = healthcareCenterId.Trim();
        Name = name.Trim();
        PaternalSurname = paternalSurname.Trim();
        MaternalSurname = maternalSurname.Trim();
        IdentityType = identityType.Trim();
        IdentityNumber = identityNumber.Trim();
        BirthDate = birthDate;
        Email = email.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        Gender = gender.Trim();
        IsActive = isActive;
        Address = address.Trim();
        Role = role.Trim();
    }
}
