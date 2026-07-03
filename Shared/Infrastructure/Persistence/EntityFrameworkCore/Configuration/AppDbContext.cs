using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Billing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaliaBackend.Iam.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context for Vitalia
/// </summary>
/// <param name="options">
///     The options for the database context
/// </param>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<BillingClaim> BillingClaims { get; set; }
    public DbSet<HealthcareCenter> HealthcareCenters { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<AppointmentFee> AppointmentFees { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Diagnosis> Diagnoses { get; set; }
    public DbSet<Treatment> Treatments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }
    public DbSet<BranchMedicine> BranchMedicines { get; set; }
    public DbSet<MedicineRestock> MedicineRestocks { get; set; }
    public DbSet<MedicalOrder> MedicalOrders { get; set; }
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <summary>
    ///     On creating the database model
    /// </summary>
    /// <remarks>
    ///     This method is used to create the database model for the application.
    /// </remarks>
    /// <param name="builder">
    ///     The model builder for the database context
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplySchedulingConfiguration();
        builder.ApplyPharmacyConfiguration();
        builder.ApplyBillingConfiguration();
        builder.ApplyTenantConfiguration();
        ApplyIamConfiguration(builder);
        builder.ApplyClinicalConfiguration();
        ApplyPartOneAdditionalConfiguration(builder);
        // General Naming Convention for the database objects
        builder.UseSnakeCaseNamingConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");

        builder.Properties<TimeOnly>()
            .HaveConversion<TimeOnlyConverter>()
            .HaveColumnType("time");

        base.ConfigureConventions(builder);
    }

    private static void ApplyIamConfiguration(ModelBuilder builder)
    {
        builder.Entity<User>().ToTable("users");
        builder.Entity<User>().HasKey(user => user.Id);
        builder.Entity<User>().Property(user => user.Id).ValueGeneratedNever();
        builder.Entity<User>().Property(user => user.HealthcareCenterId).IsRequired().HasMaxLength(36);
        builder.Entity<User>().Property(user => user.Name).IsRequired().HasMaxLength(120);
        builder.Entity<User>().Property(user => user.PaternalSurname).IsRequired().HasMaxLength(120);
        builder.Entity<User>().Property(user => user.MaternalSurname).HasMaxLength(120);
        builder.Entity<User>().Property(user => user.IdentityType).IsRequired().HasMaxLength(20);
        builder.Entity<User>().Property(user => user.IdentityNumber).IsRequired().HasMaxLength(32);
        builder.Entity<User>().Property(user => user.Email).IsRequired().HasMaxLength(160);
        builder.Entity<User>().Property(user => user.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Entity<User>().Property(user => user.Phone).HasMaxLength(32);
        builder.Entity<User>().Property(user => user.Gender).HasMaxLength(16);
        builder.Entity<User>().Property(user => user.Address).HasMaxLength(240);
        builder.Entity<User>().Property(user => user.Role).IsRequired().HasMaxLength(32);
        builder.Entity<User>().HasIndex(user => user.Email).IsUnique();
        builder.Entity<User>().HasIndex(user => user.IdentityNumber).IsUnique();

        builder.Entity<Doctor>().ToTable("doctors");
        builder.Entity<Doctor>().HasKey(doctor => doctor.UserId);
        builder.Entity<Doctor>().Property(doctor => doctor.UserId).ValueGeneratedNever();
        builder.Entity<Doctor>().Property(doctor => doctor.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Doctor>().Property(doctor => doctor.LicenseNumber).IsRequired().HasMaxLength(40);
        builder.Entity<Doctor>().Property(doctor => doctor.CmpNumber).IsRequired().HasMaxLength(40);
        builder.Entity<Doctor>().HasIndex(doctor => doctor.Code).IsUnique();

        builder.Entity<Patient>().ToTable("patients");
        builder.Entity<Patient>().HasKey(patient => patient.UserId);
        builder.Entity<Patient>().Property(patient => patient.UserId).ValueGeneratedNever();
        builder.Entity<Patient>().Property(patient => patient.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Patient>().Property(patient => patient.InsuranceProvider).HasMaxLength(120);
        builder.Entity<Patient>().Property(patient => patient.PolicyNumber).HasMaxLength(80);
        builder.Entity<Patient>().Property(patient => patient.EmergencyContactName).HasMaxLength(160);
        builder.Entity<Patient>().Property(patient => patient.EmergencyContactPhone).HasMaxLength(32);
        builder.Entity<Patient>().HasIndex(patient => patient.Code).IsUnique();

        builder.Entity<Speciality>().ToTable("specialities");
        builder.Entity<Speciality>().HasKey(speciality => speciality.Id);
        builder.Entity<Speciality>().Property(speciality => speciality.Id).ValueGeneratedNever();
        builder.Entity<Speciality>().Property(speciality => speciality.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Speciality>().Property(speciality => speciality.Description).IsRequired().HasMaxLength(120);
        builder.Entity<Speciality>().HasIndex(speciality => speciality.Code).IsUnique();

        builder.Entity<DoctorSpeciality>().ToTable("doctor_specialities");
        builder.Entity<DoctorSpeciality>().HasKey(doctorSpeciality => new { doctorSpeciality.DoctorId, doctorSpeciality.SpecialityId });
    }

    private static void ApplyPartOneAdditionalConfiguration(ModelBuilder builder)
    {
        builder.Entity<BranchMedicine>().ToTable("branch_medicines");
        builder.Entity<BranchMedicine>().HasKey(branchMedicine => new { branchMedicine.BranchId, branchMedicine.MedicineId });
        builder.Entity<BranchMedicine>().Property(branchMedicine => branchMedicine.BranchId).HasMaxLength(36);
        builder.Entity<BranchMedicine>().Property(branchMedicine => branchMedicine.Stock).IsRequired();
        builder.Entity<BranchMedicine>().Property(branchMedicine => branchMedicine.Price).IsRequired().HasPrecision(10, 2);

        builder.Entity<MedicineRestock>().ToTable("medicine_restocks");
        builder.Entity<MedicineRestock>().HasKey(medicineRestock => medicineRestock.Id);
        builder.Entity<MedicineRestock>().Property(medicineRestock => medicineRestock.Id).ValueGeneratedNever();
        builder.Entity<MedicineRestock>().Property(medicineRestock => medicineRestock.Code).IsRequired().HasMaxLength(20);
        builder.Entity<MedicineRestock>().Property(medicineRestock => medicineRestock.BranchId).IsRequired().HasMaxLength(36);
        builder.Entity<MedicineRestock>().Property(medicineRestock => medicineRestock.Quantity).IsRequired();
        builder.Entity<MedicineRestock>().HasIndex(medicineRestock => medicineRestock.Code).IsUnique();

        builder.Entity<MedicalOrder>().ToTable("medical_orders");
        builder.Entity<MedicalOrder>().HasKey(medicalOrder => medicalOrder.Id);
        builder.Entity<MedicalOrder>().Property(medicalOrder => medicalOrder.Id).ValueGeneratedNever();
        builder.Entity<MedicalOrder>().Property(medicalOrder => medicalOrder.Code).IsRequired().HasMaxLength(20);
        builder.Entity<MedicalOrder>().Property(medicalOrder => medicalOrder.Type).IsRequired().HasMaxLength(40);
        builder.Entity<MedicalOrder>().Property(medicalOrder => medicalOrder.Description).IsRequired().HasMaxLength(500);
        builder.Entity<MedicalOrder>().Property(medicalOrder => medicalOrder.Status).IsRequired().HasMaxLength(40);
        builder.Entity<MedicalOrder>().HasIndex(medicalOrder => medicalOrder.Code).IsUnique();
    }
}

public class DateOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        d => d.ToDateTime(TimeOnly.MinValue),
        d => DateOnly.FromDateTime(d))
    { }
}

public class TimeOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TimeOnly, TimeSpan>
{
    public TimeOnlyConverter() : base(
        t => t.ToTimeSpan(),
        t => TimeOnly.FromTimeSpan(t))
    { }
}
