using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Billing.Domain.Model.Aggregates;

namespace VitaliaBackend.Billing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Maps the <see cref="BillingClaim" /> aggregate to the columns and constraints
///     of its SQL table. This is the blueprint EF Core reads to generate migrations.
/// </summary>
public static class ModelBuilderExtensions
{
    public static void ApplyBillingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<BillingClaim>().HasKey(billingClaim => billingClaim.Id);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.Id).IsRequired().ValueGeneratedNever();

        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.Code).IsRequired().HasMaxLength(50);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.AppointmentId).IsRequired();
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.InsuranceProvider).IsRequired().HasMaxLength(120);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.PatientName).IsRequired().HasMaxLength(120);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.ProviderName).IsRequired().HasMaxLength(120);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.Value).IsRequired().HasPrecision(10, 2);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.ClinicalCompliance).IsRequired().HasMaxLength(30);
        builder.Entity<BillingClaim>().Property(billingClaim => billingClaim.CycleStatus).IsRequired().HasMaxLength(30);

        builder.Entity<BillingClaim>()
            .HasIndex(billingClaim => billingClaim.Code)
            .IsUnique();
    }
}
