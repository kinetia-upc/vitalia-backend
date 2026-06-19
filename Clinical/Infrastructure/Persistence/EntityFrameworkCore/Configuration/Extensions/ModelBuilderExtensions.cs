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

        builder.Entity<Diagnosis>().HasKey(diagnosis => diagnosis.Id);
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.MedicalRecordId).IsRequired().HasMaxLength(50);
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.Description).IsRequired().HasMaxLength(300);

        builder.Entity<Treatment>().HasKey(treatment => treatment.Id);
        builder.Entity<Treatment>().Property(treatment => treatment.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Treatment>().Property(treatment => treatment.MedicalRecordId).IsRequired().HasMaxLength(50);
        builder.Entity<Treatment>().Property(treatment => treatment.Description).IsRequired().HasMaxLength(300);

        builder.Entity<Prescription>().HasKey(prescription => prescription.Id);
        builder.Entity<Prescription>().Property(prescription => prescription.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Prescription>().Property(prescription => prescription.MedicalRecordId).IsRequired().HasMaxLength(50);

        builder.Entity<PrescriptionDetail>().HasKey(prescriptionDetail => prescriptionDetail.Id);
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.PrescriptionId).IsRequired();
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.MedicineId);
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.MedicineName).HasMaxLength(120);
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.Frequency).IsRequired().HasMaxLength(40);
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.Duration).IsRequired().HasMaxLength(40);
        builder.Entity<PrescriptionDetail>().OwnsOne(prescriptionDetail => prescriptionDetail.Dose, dose =>
        {
            dose.WithOwner().HasForeignKey("Id");
            dose.Property(d => d.Amount).HasColumnName("dose").IsRequired();
            dose.Property(d => d.Unit).HasColumnName("dose_unit_type").HasConversion<string>().IsRequired().HasMaxLength(20);
        });
    }
}
