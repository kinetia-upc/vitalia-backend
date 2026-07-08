using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Iam.Domain.Model.Aggregates;
using VitaliaBackend.Iam.Infrastructure.Security;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Billing.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, bool clearExisting = false)
    {
        try
        {
            if (clearExisting)
            {
                Console.WriteLine("[DbSeeder] Clearing existing data from tables...");
                await context.PrescriptionDetails.ExecuteDeleteAsync();
                await context.MedicalOrders.ExecuteDeleteAsync();
                await context.Prescriptions.ExecuteDeleteAsync();
                await context.Treatments.ExecuteDeleteAsync();
                await context.Diagnoses.ExecuteDeleteAsync();
                await context.MedicalRecords.ExecuteDeleteAsync();
                await context.Appointments.ExecuteDeleteAsync();
                await context.AvailabilitySlots.ExecuteDeleteAsync();
                await context.MedicineRestocks.ExecuteDeleteAsync();
                await context.BranchMedicines.ExecuteDeleteAsync();
                await context.Medicines.ExecuteDeleteAsync();
                await context.BillingClaims.ExecuteDeleteAsync();
                await context.DoctorSpecialities.ExecuteDeleteAsync();
                await context.Doctors.ExecuteDeleteAsync();
                await context.Patients.ExecuteDeleteAsync();
                await context.Specialities.ExecuteDeleteAsync();
                await context.Users.ExecuteDeleteAsync();
                await context.AppointmentFees.ExecuteDeleteAsync();
                await context.Branches.ExecuteDeleteAsync();
                await context.HealthcareCenters.ExecuteDeleteAsync();
                Console.WriteLine("[DbSeeder] Existing data cleared.");
            }

            // 1. Locate backend db.json. The frontend copy is no longer the source of truth.
            string path = Path.Combine(AppContext.BaseDirectory, "db.json");
            
            // Fallback paths for different execution styles
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "db.json");
            }
            if (!File.Exists(path))
            {
                path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "db.json");
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"[DbSeeder] WARNING: db.json not found. Searched path: {path}. Skipping seeding.");
                return;
            }

            Console.WriteLine($"[DbSeeder] Reading db.json from: {path}");
            string json = await File.ReadAllTextAsync(path);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Seed Users
            if (!context.Users.Any() && root.TryGetProperty("users", out var usersProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Users...");
                foreach (var item in usersProp.EnumerateArray())
                {
                    var id = Guid.Parse(item.GetProperty("id").GetString() ?? Guid.NewGuid().ToString());
                    var healthcareCenterId = item.GetProperty("healthcareCenterId").GetString() ?? "";
                    var name = item.GetProperty("name").GetString() ?? "";
                    var paternalSurname = item.GetProperty("paternalSurname").GetString() ?? "";
                    var maternalSurname = item.GetProperty("maternalSurname").GetString() ?? "";
                    var identityType = item.GetProperty("identityType").GetString() ?? "";
                    var identityNumber = item.GetProperty("identityNumber").GetString() ?? "";
                    var birthDate = DateOnly.Parse(item.GetProperty("birthDate").GetString() ?? "1900-01-01");
                    var email = item.GetProperty("email").GetString() ?? "";
                    var password = item.TryGetProperty("password", out var passwordProp)
                        ? passwordProp.GetString() ?? "Password123!"
                        : "Password123!";
                    var phone = item.GetProperty("phone").GetString() ?? "";
                    var gender = item.GetProperty("gender").GetString() ?? "";
                    var isActive = item.GetProperty("isActive").GetBoolean();
                    var address = item.GetProperty("address").GetString() ?? "";
                    var role = item.GetProperty("role").GetString() ?? "patient";

                    context.Users.Add(new User(
                        id,
                        healthcareCenterId,
                        name,
                        paternalSurname,
                        maternalSurname,
                        identityType,
                        identityNumber,
                        birthDate,
                        email,
                        PasswordHashingService.Hash(password),
                        phone,
                        gender,
                        isActive,
                        address,
                        role));
                }

                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Users successfully.");
            }

            // Seed Doctors
            if (!context.Doctors.Any() && root.TryGetProperty("doctors", out var doctorsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Doctors...");
                foreach (var item in doctorsProp.EnumerateArray())
                {
                    var userId = Guid.Parse(item.GetProperty("userId").GetString() ?? Guid.Empty.ToString());
                    var code = item.GetProperty("code").GetString() ?? "";
                    var licenseNumber = item.TryGetProperty("licenseNumber", out var licenseProp)
                        ? licenseProp.GetString() ?? ""
                        : item.GetProperty("licNumber").GetString() ?? "";
                    var cmpNumber = item.GetProperty("cmpNumber").GetString() ?? "";
                    context.Doctors.Add(new Doctor(userId, code, licenseNumber, cmpNumber));
                }

                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Doctors successfully.");
            }

            // Seed Patients
            if (!context.Patients.Any() && root.TryGetProperty("patients", out var patientsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Patients...");
                foreach (var item in patientsProp.EnumerateArray())
                {
                    var userId = Guid.Parse(item.GetProperty("userId").GetString() ?? Guid.Empty.ToString());
                    var code = item.GetProperty("code").GetString() ?? "";
                    var ehrCode = item.TryGetProperty("ehrCode", out var ehrCodeProp)
                        ? ehrCodeProp.GetString()
                        : null;
                    var insuranceProvider = item.GetProperty("insuranceProvider").GetString() ?? "";
                    var policyNumber = item.GetProperty("policyNumber").GetString() ?? "";
                    var activeThru = item.TryGetProperty("activeThru", out var activeThruProp) && activeThruProp.ValueKind == JsonValueKind.String
                        ? DateOnly.Parse(activeThruProp.GetString()!)
                        : (DateOnly?)null;
                    var emergencyContactName = item.GetProperty("emergencyContactName").GetString() ?? "";
                    var emergencyContactPhone = item.GetProperty("emergencyContactPhone").GetString() ?? "";
                    context.Patients.Add(new Patient(userId, code, insuranceProvider, policyNumber, activeThru, emergencyContactName, emergencyContactPhone, ehrCode));
                }

                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Patients successfully.");
            }

            // Seed Specialities
            if (!context.Specialities.Any() && root.TryGetProperty("specialities", out var specialitiesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Specialities...");
                foreach (var item in specialitiesProp.EnumerateArray())
                {
                    var id = Guid.Parse(item.GetProperty("id").GetString() ?? Guid.NewGuid().ToString());
                    var code = item.GetProperty("code").GetString() ?? "";
                    var description = item.GetProperty("description").GetString() ?? "";
                    context.Specialities.Add(new Speciality(id, code, description));
                }

                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Specialities successfully.");
            }

            // Seed DoctorSpecialities
            if (!context.DoctorSpecialities.Any() && root.TryGetProperty("doctorSpecialities", out var doctorSpecialitiesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Doctor Specialities...");
                foreach (var item in doctorSpecialitiesProp.EnumerateArray())
                {
                    var doctorId = Guid.Parse(item.GetProperty("doctorId").GetString() ?? Guid.Empty.ToString());
                    var specialityId = Guid.Parse(item.GetProperty("specialityId").GetString() ?? Guid.Empty.ToString());
                    context.DoctorSpecialities.Add(new DoctorSpeciality(doctorId, specialityId));
                }

                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Doctor Specialities successfully.");
            }

            // Seed Medicines
            if (!context.Medicines.Any() && root.TryGetProperty("medicines", out var medicinesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Medicines...");
                foreach (var item in medicinesProp.EnumerateArray())
                {
                    var name = item.GetProperty("name").GetString() ?? "";
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var code = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : "";
                    var unitQuantity = item.GetProperty("unitQuantity").GetInt32();
                    var unitType = item.GetProperty("unitType").GetString() ?? "mg";

                    context.Medicines.Add(new Medicine(id, code, name, unitQuantity, unitType));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Medicines successfully.");
            }

            // Seed BranchMedicines
            if (!context.BranchMedicines.Any() && root.TryGetProperty("branchMedicines", out var branchMedicinesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Branch Medicines...");
                foreach (var item in branchMedicinesProp.EnumerateArray())
                {
                    var branchId = item.GetProperty("branchId").GetString() ?? "";
                    var medicineId = Guid.Parse(item.GetProperty("medicineId").GetString() ?? Guid.Empty.ToString());
                    var stock = item.GetProperty("stock").GetInt32();
                    var price = item.GetProperty("price").GetDecimal();
                    context.BranchMedicines.Add(new BranchMedicine(branchId, medicineId, stock, price));
                }

                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Branch Medicines successfully.");
            }

            // Seed MedicineRestocks as history only. BranchMedicine stock already contains current stock.
            if (!context.MedicineRestocks.Any() && root.TryGetProperty("medicineRestocks", out var medicineRestocksProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Medicine Restocks...");
                var restockTimestamps = new List<(Guid Id, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt)>();
                foreach (var item in medicineRestocksProp.EnumerateArray())
                {
                    var id = Guid.Parse(item.GetProperty("id").GetString() ?? Guid.NewGuid().ToString());
                    var code = item.GetProperty("code").GetString() ?? "";
                    var branchId = item.GetProperty("branchId").GetString() ?? "";
                    var medicineId = Guid.Parse(item.GetProperty("medicineId").GetString() ?? Guid.Empty.ToString());
                    var quantity = item.GetProperty("quantity").GetInt32();
                    var createdByUserId = Guid.Parse(item.GetProperty("createdByUserId").GetString() ?? Guid.Empty.ToString());
                    context.MedicineRestocks.Add(new MedicineRestock(id, code, branchId, medicineId, quantity, createdByUserId));
                    var restockCreatedAt = ParseAuditTimestamp(item, "createdAt");
                    restockTimestamps.Add((id, restockCreatedAt, restockCreatedAt));
                }

                await context.SaveChangesAsync();
                foreach (var (id, createdAt, updatedAt) in restockTimestamps)
                    await StampAuditTimestampsAsync(context, "medicine_restocks", id, createdAt, updatedAt);
                Console.WriteLine("[DbSeeder] Seeded Medicine Restocks successfully.");
            }

            var appointmentIdByCode = new Dictionary<string, Guid>();
            if (root.TryGetProperty("appointments", out var appointmentMapProp))
            {
                foreach (var item in appointmentMapProp.EnumerateArray())
                {
                    var appointmentId = Guid.Parse(item.GetProperty("id").GetString() ?? Guid.Empty.ToString());
                    var appointmentCode = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? string.Empty
                        : string.Empty;

                    if (!string.IsNullOrWhiteSpace(appointmentCode) && appointmentId != Guid.Empty)
                    {
                        appointmentIdByCode[appointmentCode] = appointmentId;
                    }
                }
            }

            if (appointmentIdByCode.Count > 0 && context.Appointments.Any())
            {
                Console.WriteLine("[DbSeeder] Synchronizing existing Appointment ids by code...");
                var existingAppointments = await context.Appointments
                    .AsNoTracking()
                    .Select(appointment => new { appointment.Id, appointment.Code })
                    .ToListAsync();

                foreach (var appointment in existingAppointments)
                {
                    if (!appointmentIdByCode.TryGetValue(appointment.Code, out var expectedId) || appointment.Id == expectedId)
                    {
                        continue;
                    }

                    await context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE appointments SET id = {expectedId} WHERE code = {appointment.Code}");
                }

                Console.WriteLine("[DbSeeder] Appointment id synchronization completed.");
            }

            // Seed Appointments
            if (!context.Appointments.Any() && root.TryGetProperty("appointments", out var appointmentsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Appointments...");
                foreach (var item in appointmentsProp.EnumerateArray())
                {
                    var appointmentId = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var publicId = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : item.GetProperty("id").GetString() ?? "";
                    var doctorId = item.GetProperty("doctorId").GetString() ?? "";
                    var patientId = item.GetProperty("patientId").GetString() ?? "";
                    var branchId = item.GetProperty("branchId").GetString() ?? "";
                    var scheduledAt = DateTime.Parse(item.GetProperty("scheduledAt").GetString() ?? DateTime.Now.ToString());
                    var reason = item.GetProperty("reason").GetString() ?? "";
                    
                    var statusStr = item.GetProperty("status").GetString() ?? "scheduled";
                    var status = Enum.TryParse<EAppointmentStatus>(statusStr, true, out var parsedStatus) ? parsedStatus : EAppointmentStatus.Scheduled;

                    var paymentStr = item.GetProperty("paymentStatus").GetString() ?? "pending";
                    var paymentStatus = Enum.TryParse<EPaymentStatus>(paymentStr, true, out var parsedPayment) ? parsedPayment : EPaymentStatus.Pending;

                    context.Appointments.Add(new Appointment(
                        appointmentId,
                        publicId,
                        Guid.Parse(doctorId),
                        Guid.Parse(patientId),
                        branchId,
                        scheduledAt,
                        reason,
                        status,
                        paymentStatus));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Appointments successfully.");
            }

            // Seed AvailabilitySlots
            if (!context.AvailabilitySlots.Any() && root.TryGetProperty("availabilitySlots", out var slotsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Availability Slots...");
                foreach (var item in slotsProp.EnumerateArray())
                {
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var publicId = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : item.GetProperty("id").GetString() ?? "";
                    var doctorId = item.GetProperty("doctorId").GetString() ?? "";
                    var branchId = item.GetProperty("branchId").GetString() ?? "";
                    var date = DateOnly.Parse(item.GetProperty("date").GetString() ?? "");
                    var startTime = TimeOnly.Parse(item.GetProperty("startTime").GetString() ?? "");
                    var endTime = TimeOnly.Parse(item.GetProperty("endTime").GetString() ?? "");

                    var statusStr = item.GetProperty("status").GetString() ?? "available";
                    var status = Enum.TryParse<EAvailabilitySlotStatus>(statusStr, true, out var parsedStatus) ? parsedStatus : EAvailabilitySlotStatus.Available;

                    context.AvailabilitySlots.Add(new AvailabilitySlot(id, publicId, Guid.Parse(doctorId), branchId, date, startTime, endTime, status));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Availability Slots successfully.");
            }

            // Seed MedicalRecords
            var medicalRecordCodeMap = new Dictionary<string, string>();
            var medicalRecordIdMap = new Dictionary<string, Guid>();
            if (!context.MedicalRecords.Any() && root.TryGetProperty("medicalRecords", out var mrProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Medical Records...");
                foreach (var item in mrProp.EnumerateArray())
                {
                    var mockId = item.GetProperty("id").GetString() ?? "";
                    var code = item.GetProperty("code").GetString() ?? mockId;
                    var patientId = Guid.Parse(item.GetProperty("patientId").GetString() ?? Guid.Empty.ToString());
                    
                    var apptProp = item.GetProperty("appointmentId");
                    var appointmentId = apptProp.ValueKind != JsonValueKind.Null
                        ? Guid.Parse(apptProp.GetString() ?? Guid.Empty.ToString())
                        : Guid.Empty;

                    var mrId = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var mr = new MedicalRecord(mrId, code, appointmentId, patientId);

                    context.MedicalRecords.Add(mr);
                    await context.SaveChangesAsync();
                    await StampAuditTimestampsAsync(
                        context,
                        "medical_records",
                        mr.Id,
                        ParseAuditTimestamp(item, "createdAt"),
                        ParseAuditTimestamp(item, "updatedAt"));
                    medicalRecordCodeMap[mockId] = mr.Code;
                    medicalRecordIdMap[mockId] = mr.Id;
                }
                Console.WriteLine("[DbSeeder] Seeded Medical Records successfully.");
            }

            // Seed Diagnoses
            if (!context.Diagnoses.Any() && root.TryGetProperty("diagnoses", out var diagnosesProp) && medicalRecordIdMap.Count > 0)
            {
                Console.WriteLine("[DbSeeder] Seeding Diagnoses...");
                var diagnosisTimestamps = new List<(Guid Id, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt)>();
                foreach (var item in diagnosesProp.EnumerateArray())
                {
                    var mockMrId = item.GetProperty("medicalRecordId").GetString() ?? "";
                    if (!medicalRecordIdMap.TryGetValue(mockMrId, out var medicalRecordId))
                    {
                        continue;
                    }
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var code = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : "";
                    var description = item.GetProperty("description").GetString() ?? "";

                    var cie10Code = item.TryGetProperty("cie10Code", out var cie10CodeProp)
                        ? cie10CodeProp.GetString() ?? ""
                        : "";
                    var source = item.TryGetProperty("diagnosisCatalogSource", out var sourceProp) &&
                                 Enum.TryParse<DiagnosisCatalogSource>(sourceProp.GetString(), true, out var parsedSource)
                        ? parsedSource
                        : DiagnosisCatalogSource.MINSA_CIE10;

                    context.Diagnoses.Add(new Diagnosis(id, code, medicalRecordId, cie10Code, description, source));
                    diagnosisTimestamps.Add((id, ParseAuditTimestamp(item, "createdAt"), ParseAuditTimestamp(item, "updatedAt")));
                }
                await context.SaveChangesAsync();
                foreach (var (id, createdAt, updatedAt) in diagnosisTimestamps)
                    await StampAuditTimestampsAsync(context, "diagnoses", id, createdAt, updatedAt);
                Console.WriteLine("[DbSeeder] Seeded Diagnoses successfully.");
            }

            // Seed Treatments
            if (!context.Treatments.Any() && root.TryGetProperty("treatments", out var treatmentsProp) && medicalRecordIdMap.Count > 0)
            {
                Console.WriteLine("[DbSeeder] Seeding Treatments...");
                var treatmentTimestamps = new List<(Guid Id, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt)>();
                foreach (var item in treatmentsProp.EnumerateArray())
                {
                    var mockMrId = item.GetProperty("medicalRecordId").GetString() ?? "";
                    if (!medicalRecordIdMap.TryGetValue(mockMrId, out var medicalRecordId))
                    {
                        continue;
                    }
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var code = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : "";
                    var description = item.GetProperty("description").GetString() ?? "";

                    context.Treatments.Add(new Treatment(id, code, medicalRecordId, description));
                    treatmentTimestamps.Add((id, ParseAuditTimestamp(item, "createdAt"), ParseAuditTimestamp(item, "updatedAt")));
                }
                await context.SaveChangesAsync();
                foreach (var (id, createdAt, updatedAt) in treatmentTimestamps)
                    await StampAuditTimestampsAsync(context, "treatments", id, createdAt, updatedAt);
                Console.WriteLine("[DbSeeder] Seeded Treatments successfully.");
            }

            // Seed Prescriptions
            var prescriptionIdMap = new Dictionary<string, Guid>();
            if (!context.Prescriptions.Any() && root.TryGetProperty("prescriptions", out var prescriptionsProp) && medicalRecordIdMap.Count > 0)
            {
                Console.WriteLine("[DbSeeder] Seeding Prescriptions...");
                foreach (var item in prescriptionsProp.EnumerateArray())
                {
                    var mockId = item.GetProperty("id").GetString() ?? "";
                    var mockMrId = item.GetProperty("medicalRecordId").GetString() ?? "";
                    if (!medicalRecordCodeMap.TryGetValue(mockMrId, out var actualMrCode))
                    {
                        continue;
                    }

                    var prsId = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var code = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? $"PRS-{Random.Shared.Next(10000, 100000)}"
                        : $"PRS-{Random.Shared.Next(10000, 100000)}";

                    var prescription = new Prescription(prsId, code, medicalRecordIdMap[mockMrId]);
                    context.Prescriptions.Add(prescription);

                    await context.SaveChangesAsync();
                    await StampAuditTimestampsAsync(
                        context,
                        "prescriptions",
                        prescription.Id,
                        ParseAuditTimestamp(item, "createdAt"),
                        ParseAuditTimestamp(item, "updatedAt"));
                    prescriptionIdMap[mockId] = prescription.Id;
                }
                Console.WriteLine("[DbSeeder] Seeded Prescriptions successfully.");
            }

            // Seed PrescriptionDetails
            if (!context.PrescriptionDetails.Any() && root.TryGetProperty("prescriptionDetails", out var pdProp) && prescriptionIdMap.Count > 0)
            {
                Console.WriteLine("[DbSeeder] Seeding Prescription Details...");
                var prescriptionDetailTimestamps = new List<(Guid PrescriptionId, Guid MedicineId, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt)>();
                foreach (var item in pdProp.EnumerateArray())
                {
                    var mockPrescriptionId = item.GetProperty("prescriptionId").GetString() ?? "";
                    if (!prescriptionIdMap.TryGetValue(mockPrescriptionId, out var prescriptionId))
                    {
                        continue;
                    }

                    var medicineId = Guid.Parse(item.GetProperty("medicineId").GetString() ?? Guid.Empty.ToString());
                    var quantity = item.GetProperty("quantity").GetInt32();
                    var frequency = item.GetProperty("frequency").GetInt32();
                    var duration = item.GetProperty("duration").GetInt32();

                    context.PrescriptionDetails.Add(new PrescriptionDetail(prescriptionId, medicineId, quantity, frequency, duration));
                    prescriptionDetailTimestamps.Add((prescriptionId, medicineId, ParseAuditTimestamp(item, "createdAt"), ParseAuditTimestamp(item, "updatedAt")));
                }
                await context.SaveChangesAsync();
                foreach (var (prescriptionId, medicineId, createdAt, updatedAt) in prescriptionDetailTimestamps)
                    await StampPrescriptionDetailTimestampsAsync(context, prescriptionId, medicineId, createdAt, updatedAt);
                Console.WriteLine("[DbSeeder] Seeded Prescription Details successfully.");
            }

            // Seed Billing Claims
            if (!context.BillingClaims.Any() && root.TryGetProperty("billingClaims", out var billingClaimsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Billing Claims...");
                foreach (var item in billingClaimsProp.EnumerateArray())
                {
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var claimCode = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : item.GetProperty("claimCode").GetString() ?? "";
                    var insuranceProvider = item.GetProperty("insuranceProvider").GetString() ?? "";
                    var appointmentId = item.TryGetProperty("appointmentId", out var appointmentIdProp)
                        ? Guid.Parse(appointmentIdProp.GetString() ?? Guid.Empty.ToString())
                        : Guid.Empty;
                    var patientName = item.TryGetProperty("patientName", out var patientNameProp)
                        ? patientNameProp.GetString() ?? ""
                        : "";
                    var providerName = item.GetProperty("providerName").GetString() ?? "";
                    var value = item.GetProperty("value").GetDecimal();
                    var clinicalCompliance = item.GetProperty("clinicalCompliance").GetString() ?? "";
                    var cycleStatus = item.GetProperty("cycleStatus").GetString() ?? "";

                    context.BillingClaims.Add(new BillingClaim(
                        id,
                        claimCode,
                        appointmentId,
                        insuranceProvider,
                        patientName,
                        providerName,
                        value,
                        clinicalCompliance,
                        cycleStatus));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Billing Claims successfully.");
            }

            // Seed MedicalOrders
            if (!context.MedicalOrders.Any() && root.TryGetProperty("medicalOrders", out var medicalOrdersProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Medical Orders...");
                var medicalOrderTimestamps = new List<(Guid Id, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt)>();
                foreach (var item in medicalOrdersProp.EnumerateArray())
                {
                    var id = Guid.Parse(item.GetProperty("id").GetString() ?? Guid.NewGuid().ToString());
                    var code = item.GetProperty("code").GetString() ?? "";
                    var patientId = Guid.Parse(item.GetProperty("patientId").GetString() ?? Guid.Empty.ToString());
                    var doctorId = Guid.Parse(item.GetProperty("doctorId").GetString() ?? Guid.Empty.ToString());
                    var appointmentId = Guid.Parse(item.GetProperty("appointmentId").GetString() ?? Guid.Empty.ToString());
                    var medicalRecordId = item.TryGetProperty("medicalRecordId", out var medicalRecordIdProp) && medicalRecordIdProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(medicalRecordIdProp.GetString()!)
                        : (Guid?)null;
                    var type = item.GetProperty("type").GetString() ?? "other";
                    var description = item.GetProperty("description").GetString() ?? "";
                    var status = item.GetProperty("status").GetString() ?? "pending";
                    context.MedicalOrders.Add(new MedicalOrder(id, code, patientId, doctorId, appointmentId, medicalRecordId, type, description, status));
                    medicalOrderTimestamps.Add((id, ParseAuditTimestamp(item, "createdAt"), ParseAuditTimestamp(item, "updatedAt")));
                }

                await context.SaveChangesAsync();
                foreach (var (id, createdAt, updatedAt) in medicalOrderTimestamps)
                    await StampAuditTimestampsAsync(context, "medical_orders", id, createdAt, updatedAt);
                Console.WriteLine("[DbSeeder] Seeded Medical Orders successfully.");
            }

            // Seed Healthcare Centers
            if (!context.HealthcareCenters.Any() && root.TryGetProperty("healthcareCenters", out var healthcareCentersProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Healthcare Centers...");
                foreach (var item in healthcareCentersProp.EnumerateArray())
                {
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var publicId = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : item.GetProperty("id").GetString() ?? "";
                    var name = item.GetProperty("healthcareCenterName").GetString() ?? "";

                    var startProp = item.GetProperty("allianceStartDate");
                    var allianceStartDate = startProp.ValueKind == JsonValueKind.String
                        ? DateOnly.Parse(startProp.GetString()!) : (DateOnly?)null;

                    var finishProp = item.GetProperty("allianceFinishDate");
                    var allianceFinishDate = finishProp.ValueKind == JsonValueKind.String
                        ? DateOnly.Parse(finishProp.GetString()!) : (DateOnly?)null;

                    var rucProp = item.GetProperty("rucNumber");
                    var rucNumber = rucProp.ValueKind switch
                    {
                        JsonValueKind.String => rucProp.GetString(),
                        JsonValueKind.Number => rucProp.GetInt64().ToString(),
                        _ => null
                    };

                    context.HealthcareCenters.Add(new HealthcareCenter(id, publicId, name, allianceStartDate, allianceFinishDate, rucNumber));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Healthcare Centers successfully.");
            }

            // Seed Branches
            if (!context.Branches.Any() && root.TryGetProperty("branches", out var branchesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Branches...");
                foreach (var item in branchesProp.EnumerateArray())
                {
                    var id = Guid.Parse(item.GetProperty("id").GetString() ?? "");
                    var publicId = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : item.GetProperty("id").GetString() ?? "";
                    var healthcareCenterId = item.GetProperty("healthcareCenterId").GetString() ?? "";

                    var branchName = item.GetProperty("branchName").GetString() ?? "";
                    var address = item.GetProperty("address").GetString() ?? "";
                    var diagnosisCatalogSource = item.TryGetProperty("diagnosisCatalogSource", out var sourceProp) &&
                                                  Enum.TryParse<DiagnosisCatalogSource>(sourceProp.GetString(), true, out var parsedSource)
                        ? parsedSource
                        : DiagnosisCatalogSource.MINSA_CIE10;

                    context.Branches.Add(new Branch(id, publicId, healthcareCenterId, branchName, address, diagnosisCatalogSource));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Branches successfully.");
            }

            await SeedDiagnosisCatalogAsync(context);

            // Seed Appointment Fees
            if (!context.AppointmentFees.Any() && root.TryGetProperty("appointmentFees", out var appointmentFeesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Appointment Fees...");
                foreach (var item in appointmentFeesProp.EnumerateArray())
                {
                    var id = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                        ? Guid.Parse(idProp.GetString()!)
                        : Guid.NewGuid();
                    var publicId = item.TryGetProperty("code", out var codeProp)
                        ? codeProp.GetString() ?? ""
                        : item.GetProperty("id").GetString() ?? "";
                    var branchId = item.GetProperty("branchId").GetString() ?? "";

                    var specialityIdProp = item.GetProperty("specialityId");
                    var specialityId = specialityIdProp.ValueKind == JsonValueKind.String ? specialityIdProp.GetString() : null;

                    var price = item.GetProperty("price").GetDecimal();

                    context.AppointmentFees.Add(new AppointmentFee(id, publicId, branchId, specialityId, price));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Appointment Fees successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] ERROR: Failed to seed data from db.json. Details: {ex.Message}");
        }
    }

    private static DateTimeOffset? ParseAuditTimestamp(JsonElement item, string propertyName)
    {
        return item.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
            ? DateTimeOffset.Parse(prop.GetString()!)
            : null;
    }

    /// <summary>
    ///     Overwrites the CreatedAt/UpdatedAt audit columns for a just-seeded row via raw SQL,
    ///     since <see cref="Interceptors.AuditableEntityInterceptor" /> always stamps them with the
    ///     current time on insert through the normal EF change tracker.
    /// </summary>
    private static async Task StampAuditTimestampsAsync(
        AppDbContext context,
        string tableName,
        Guid id,
        DateTimeOffset? createdAt,
        DateTimeOffset? updatedAt)
    {
        if (createdAt is null && updatedAt is null) return;

        await context.Database.ExecuteSqlRawAsync(
            $"UPDATE {tableName} SET created_at = {{0}}, updated_at = {{1}} WHERE id = {{2}}",
            createdAt, updatedAt, id);
    }

    /// <summary>
    ///     Same as <see cref="StampAuditTimestampsAsync" /> but for prescription_details, whose
    ///     primary key is the composite (prescription_id, medicine_id) instead of a single id column.
    /// </summary>
    private static async Task StampPrescriptionDetailTimestampsAsync(
        AppDbContext context,
        Guid prescriptionId,
        Guid medicineId,
        DateTimeOffset? createdAt,
        DateTimeOffset? updatedAt)
    {
        if (createdAt is null && updatedAt is null) return;

        await context.Database.ExecuteSqlRawAsync(
            "UPDATE prescription_details SET created_at = {0}, updated_at = {1} WHERE prescription_id = {2} AND medicine_id = {3}",
            createdAt, updatedAt, prescriptionId, medicineId);
    }

    private static async Task SeedDiagnosisCatalogAsync(AppDbContext context)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "minsa-cie10.csv");

        if (!File.Exists(path))
            path = Path.Combine(Directory.GetCurrentDirectory(), "minsa-cie10.csv");

        if (!File.Exists(path))
            path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "minsa-cie10.csv");

        if (!File.Exists(path))
            return;

        var source = DiagnosisCatalogSource.MINSA_CIE10;
        var alreadySeeded = await context.DiagnosisCatalogEntries
            .AnyAsync(entry => entry.Source == source);

        if (alreadySeeded)
        {
            Console.WriteLine("[DbSeeder] MINSA CIE-10 catalog import completed.");
            return;
        }

        Console.WriteLine($"[DbSeeder] Importing MINSA CIE-10 catalog from: {path}");

        using var reader = await DiagnosisCatalogCsvEncoding.CreateReaderAsync(path);
        var lineNumber = 0;
        var entries = new List<DiagnosisCatalogEntry>();

        while (await reader.ReadLineAsync() is { } line)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = ParseCsvLine(line);
            if (columns.Count < 2)
                continue;

            if (lineNumber == 1 &&
                columns[0].Trim().Equals("code", StringComparison.OrdinalIgnoreCase) &&
                columns[1].Trim().Equals("description", StringComparison.OrdinalIgnoreCase))
                continue;

            var code = columns[0].Trim().ToUpperInvariant();
            var description = columns[1].Trim();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(description))
                continue;

            var searchText = DiagnosisCatalogSearchNormalizer.NormalizeForSearch($"{code} {description}");

            entries.Add(new DiagnosisCatalogEntry(
                Guid.NewGuid(),
                source,
                code,
                description,
                searchText));
        }

        context.DiagnosisCatalogEntries.AddRange(entries);
        await context.SaveChangesAsync();
        Console.WriteLine("[DbSeeder] MINSA CIE-10 catalog import completed.");
    }

    private static List<string> ParseCsvLine(string line)
    {
        var columns = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];

            if (character == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    current.Append('"');
                    index++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (character == ',' && !inQuotes)
            {
                columns.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(character);
        }

        columns.Add(current.ToString());
        return columns;
    }
}
