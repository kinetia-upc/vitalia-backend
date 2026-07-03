using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyClinicalConfiguration(this ModelBuilder builder)
    {
        builder.Entity<MedicalRecord>().HasKey(medicalRecord => medicalRecord.Id);
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.Id).IsRequired().ValueGeneratedNever();
        builder.Entity<MedicalRecord>().HasIndex(medicalRecord => medicalRecord.Code).IsUnique();
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.Code).IsRequired().HasMaxLength(50);
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.PatientId).IsRequired();
        builder.Entity<MedicalRecord>().Property(medicalRecord => medicalRecord.AppointmentId).IsRequired();

        builder.Entity<MedicalRecord>()
            .HasIndex(medicalRecord => new { medicalRecord.PatientId, medicalRecord.AppointmentId });

        builder.Entity<Diagnosis>().HasKey(diagnosis => diagnosis.Id);
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.Id).IsRequired().ValueGeneratedNever();
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.MedicalRecordId).IsRequired();
        builder.Entity<Diagnosis>().Property(diagnosis => diagnosis.Description).IsRequired().HasMaxLength(300);
        builder.Entity<Diagnosis>().HasIndex(diagnosis => diagnosis.Code).IsUnique();

        builder.Entity<Treatment>().HasKey(treatment => treatment.Id);
        builder.Entity<Treatment>().Property(treatment => treatment.Id).IsRequired().ValueGeneratedNever();
        builder.Entity<Treatment>().Property(treatment => treatment.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Treatment>().Property(treatment => treatment.MedicalRecordId).IsRequired();
        builder.Entity<Treatment>().Property(treatment => treatment.Description).IsRequired().HasMaxLength(300);
        builder.Entity<Treatment>().HasIndex(treatment => treatment.Code).IsUnique();

        builder.Entity<Prescription>().HasKey(prescription => prescription.Id);
        builder.Entity<Prescription>().Property(prescription => prescription.Id).IsRequired().ValueGeneratedNever();
        builder.Entity<Prescription>().Property(prescription => prescription.Code).IsRequired().HasMaxLength(20);
        builder.Entity<Prescription>().Property(prescription => prescription.MedicalRecordId).IsRequired();
        builder.Entity<Prescription>().HasIndex(prescription => prescription.Code).IsUnique();

        builder.Entity<PrescriptionDetail>().HasKey(prescriptionDetail => new { prescriptionDetail.PrescriptionId, prescriptionDetail.MedicineId });
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.PrescriptionId).IsRequired();
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.MedicineId).IsRequired();
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.Quantity).IsRequired();
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.Frequency).IsRequired();
        builder.Entity<PrescriptionDetail>().Property(prescriptionDetail => prescriptionDetail.Duration).IsRequired();
    }
}
