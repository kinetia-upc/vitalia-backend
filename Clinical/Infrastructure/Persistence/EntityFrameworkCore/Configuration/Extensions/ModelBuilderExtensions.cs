using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyClinicalConfiguration(this ModelBuilder builder)
    {
        builder.Entity<MedicalRecord>().HasKey(medicalRecord => medicalRecord.Id);
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<MedicalRecord>().HasIndex(medicalRecord => medicalRecord.Code).IsUnique();
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.Code).IsRequired().HasMaxLength(50);
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.PatientId).IsRequired().HasMaxLength(50);
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.AppointmentId).IsRequired().HasMaxLength(50);

        builder.Entity<MedicalRecord>()
            .HasIndex(medicalRecord => new { medicalRecord.PatientId, medicalRecord.AppointmentId });
    }
}
