using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Entities;

namespace VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySchedulingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Appointment>().HasKey(appointment => appointment.Id);
        builder.Entity<Appointment>().Property(appointment => appointment.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Appointment>().Property(appointment => appointment.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<Appointment>().HasIndex(appointment => appointment.PublicId).IsUnique();
        builder.Entity<Appointment>().Property(appointment => appointment.DoctorId).IsRequired().HasMaxLength(50);
        builder.Entity<Appointment>().Property(appointment => appointment.PatientId).IsRequired().HasMaxLength(50);
        builder.Entity<Appointment>().Property(appointment => appointment.BranchId).IsRequired().HasMaxLength(50);
        builder.Entity<Appointment>().Property(appointment => appointment.ScheduledAt).IsRequired();
        builder.Entity<Appointment>().Property(appointment => appointment.Reason).IsRequired().HasMaxLength(300);
        builder.Entity<Appointment>().Property(appointment => appointment.Status).IsRequired();
        builder.Entity<Appointment>().Property(appointment => appointment.PaymentStatus).IsRequired();
        
          builder.Entity<Appointment>()
            .HasIndex(appointment => new { appointment.DoctorId, appointment.ScheduledAt });

        builder.Entity<AvailabilitySlot>().HasKey(slot => slot.Id);
        builder.Entity<AvailabilitySlot>().Property(slot => slot.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AvailabilitySlot>().Property(slot => slot.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<AvailabilitySlot>().HasIndex(slot => slot.PublicId).IsUnique();
        builder.Entity<AvailabilitySlot>().Property(slot => slot.DoctorId).IsRequired().HasMaxLength(50);
        builder.Entity<AvailabilitySlot>().Property(slot => slot.BranchId).IsRequired().HasMaxLength(50);
        builder.Entity<AvailabilitySlot>().Property(slot => slot.Date).IsRequired();
        builder.Entity<AvailabilitySlot>().Property(slot => slot.StartTime).IsRequired();
        builder.Entity<AvailabilitySlot>().Property(slot => slot.EndTime).IsRequired();
        builder.Entity<AvailabilitySlot>().Property(slot => slot.Status).IsRequired();

        builder.Entity<AvailabilitySlot>()
            .HasIndex(slot => new { slot.DoctorId, slot.BranchId, slot.Date, slot.StartTime });

        builder.Entity<SchedulingDoctor>().HasKey(doctor => doctor.Id);
        builder.Entity<SchedulingDoctor>().Property(doctor => doctor.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SchedulingDoctor>().Property(doctor => doctor.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<SchedulingDoctor>().HasIndex(doctor => doctor.PublicId).IsUnique();
        builder.Entity<SchedulingDoctor>().Property(doctor => doctor.IdUser).IsRequired().HasMaxLength(50);
        builder.Entity<SchedulingDoctor>().Property(doctor => doctor.Specialty).IsRequired().HasMaxLength(100);
        builder.Entity<SchedulingDoctor>().Property(doctor => doctor.BranchId).IsRequired().HasMaxLength(50);

        builder.Entity<SchedulingPatient>().HasKey(patient => patient.Id);
        builder.Entity<SchedulingPatient>().Property(patient => patient.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SchedulingPatient>().Property(patient => patient.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<SchedulingPatient>().HasIndex(patient => patient.PublicId).IsUnique();
        builder.Entity<SchedulingPatient>().Property(patient => patient.IdUser).IsRequired().HasMaxLength(50);
        builder.Entity<SchedulingPatient>().Property(patient => patient.InsuranceProvider).IsRequired().HasMaxLength(100);

        builder.Entity<SchedulingBranch>().HasKey(branch => branch.Id);
        builder.Entity<SchedulingBranch>().Property(branch => branch.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SchedulingBranch>().Property(branch => branch.PublicId).IsRequired().HasMaxLength(50);
        builder.Entity<SchedulingBranch>().HasIndex(branch => branch.PublicId).IsUnique();
        builder.Entity<SchedulingBranch>().Property(branch => branch.Name).IsRequired().HasMaxLength(120);
        builder.Entity<SchedulingBranch>().Property(branch => branch.Description).IsRequired().HasMaxLength(300);
    }
}