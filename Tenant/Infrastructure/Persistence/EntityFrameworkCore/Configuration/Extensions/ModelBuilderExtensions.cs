using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Maps the 3 Tenant aggregates (HealthcareCenter, Branch, AppointmentFee) to the
///     columns and constraints of their SQL tables.
/// </summary>
public static class ModelBuilderExtensions
{
    public static void ApplyTenantConfiguration(this ModelBuilder builder)
    {
        builder.Entity<HealthcareCenter>().HasKey(healthcareCenter => healthcareCenter.Id);
        builder.Entity<HealthcareCenter>().Property(healthcareCenter => healthcareCenter.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<HealthcareCenter>().Property(healthcareCenter => healthcareCenter.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<HealthcareCenter>().HasIndex(healthcareCenter => healthcareCenter.PublicId).IsUnique();
        builder.Entity<HealthcareCenter>().Property(healthcareCenter => healthcareCenter.Name).IsRequired().HasMaxLength(150);
        builder.Entity<HealthcareCenter>().Property(healthcareCenter => healthcareCenter.AllianceStartDate);
        builder.Entity<HealthcareCenter>().Property(healthcareCenter => healthcareCenter.AllianceFinishDate);
        builder.Entity<HealthcareCenter>().Property(healthcareCenter => healthcareCenter.RucNumber);

        builder.Entity<Branch>().HasKey(branch => branch.Id);
        builder.Entity<Branch>().Property(branch => branch.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Branch>().Property(branch => branch.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<Branch>().HasIndex(branch => branch.PublicId).IsUnique();
        builder.Entity<Branch>().Property(branch => branch.HealthcareCenterId).IsRequired().HasMaxLength(50);
        builder.Entity<Branch>().Property(branch => branch.AddressId).HasMaxLength(50);
        builder.Entity<Branch>().Property(branch => branch.Name).IsRequired().HasMaxLength(150);
        builder.Entity<Branch>().Property(branch => branch.Address).IsRequired().HasMaxLength(250);
        builder.Entity<Branch>().HasIndex(branch => branch.HealthcareCenterId);

        builder.Entity<AppointmentFee>().HasKey(appointmentFee => appointmentFee.Id);
        builder.Entity<AppointmentFee>().Property(appointmentFee => appointmentFee.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AppointmentFee>().Property(appointmentFee => appointmentFee.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<AppointmentFee>().HasIndex(appointmentFee => appointmentFee.PublicId).IsUnique();
        builder.Entity<AppointmentFee>().Property(appointmentFee => appointmentFee.BranchId).IsRequired().HasMaxLength(50);
        builder.Entity<AppointmentFee>().Property(appointmentFee => appointmentFee.SpecialityId).HasMaxLength(50);
        builder.Entity<AppointmentFee>().Property(appointmentFee => appointmentFee.Price).IsRequired().HasPrecision(10, 2);
        builder.Entity<AppointmentFee>().HasIndex(appointmentFee => appointmentFee.BranchId);
    }
}
