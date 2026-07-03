using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

namespace VitaliaBackend.Pharmacy.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyPharmacyConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Medicine>().HasKey(medicine => medicine.Id);
        builder.Entity<Medicine>().Property(medicine => medicine.Id).IsRequired().ValueGeneratedNever();
        builder.Entity<Medicine>().Property(medicine => medicine.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Medicine>().Property(medicine => medicine.Name).IsRequired().HasMaxLength(120);
        builder.Entity<Medicine>().Property(medicine => medicine.UnitQuantity).IsRequired();
        builder.Entity<Medicine>().Property(medicine => medicine.UnitType).IsRequired().HasMaxLength(40);
        builder.Entity<Medicine>().HasIndex(medicine => medicine.Code).IsUnique();
        builder.Entity<Medicine>()
            .HasIndex(medicine => new { medicine.Name, medicine.UnitQuantity, medicine.UnitType })
            .IsUnique();
    }
}
