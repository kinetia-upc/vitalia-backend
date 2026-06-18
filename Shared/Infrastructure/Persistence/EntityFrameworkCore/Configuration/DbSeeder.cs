using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Model.ValueObjects;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;

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
                await context.Prescriptions.ExecuteDeleteAsync();
                await context.Treatments.ExecuteDeleteAsync();
                await context.Diagnoses.ExecuteDeleteAsync();
                await context.MedicalRecords.ExecuteDeleteAsync();
                await context.Appointments.ExecuteDeleteAsync();
                await context.AvailabilitySlots.ExecuteDeleteAsync();
                await context.Medicines.ExecuteDeleteAsync();
                Console.WriteLine("[DbSeeder] Existing data cleared.");
            }

            // 1. Locate db.json
            string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "vitalia-frontend", "server", "db.json");
            
            // Fallback paths for different execution styles
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "..", "vitalia-frontend", "server", "db.json");
            }
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "server", "db.json");
            }
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "db.json");
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

            // Seed Medicines
            if (!context.Medicines.Any() && root.TryGetProperty("medicines", out var medicinesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Medicines...");
                foreach (var item in medicinesProp.EnumerateArray())
                {
                    var name = item.GetProperty("name").GetString() ?? "";
                    var unitQuantity = item.GetProperty("unitQuantity").GetInt32();
                    var unitType = item.GetProperty("unitType").GetString() ?? "mg";
                    var price = item.GetProperty("price").GetDecimal();
                    var stock = item.GetProperty("stock").GetInt32();

                    context.Medicines.Add(new Medicine(name, unitQuantity, unitType, price, stock));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Medicines successfully.");
            }

            // Seed Appointments
            if (!context.Appointments.Any() && root.TryGetProperty("appointments", out var appointmentsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Appointments...");
                foreach (var item in appointmentsProp.EnumerateArray())
                {
                    var publicId = item.GetProperty("id").GetString() ?? "";
                    var doctorId = item.GetProperty("doctorId").GetString() ?? "";
                    var patientId = item.GetProperty("patientId").GetString() ?? "";
                    var branchId = item.GetProperty("branchId").GetString() ?? "";
                    var scheduledAt = DateTime.Parse(item.GetProperty("scheduledAt").GetString() ?? DateTime.Now.ToString());
                    var reason = item.GetProperty("reason").GetString() ?? "";
                    
                    var statusStr = item.GetProperty("status").GetString() ?? "scheduled";
                    var status = Enum.TryParse<EAppointmentStatus>(statusStr, true, out var parsedStatus) ? parsedStatus : EAppointmentStatus.Scheduled;

                    var paymentStr = item.GetProperty("paymentStatus").GetString() ?? "pending";
                    var paymentStatus = Enum.TryParse<EPaymentStatus>(paymentStr, true, out var parsedPayment) ? parsedPayment : EPaymentStatus.Pending;

                    context.Appointments.Add(new Appointment(publicId, doctorId, patientId, branchId, scheduledAt, reason, status, paymentStatus));
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
                    var publicId = item.GetProperty("id").GetString() ?? "";
                    var doctorId = item.GetProperty("doctorId").GetString() ?? "";
                    var branchId = item.GetProperty("branchId").GetString() ?? "";
                    var date = DateOnly.Parse(item.GetProperty("date").GetString() ?? "");
                    var startTime = TimeOnly.Parse(item.GetProperty("startTime").GetString() ?? "");
                    var endTime = TimeOnly.Parse(item.GetProperty("endTime").GetString() ?? "");

                    var statusStr = item.GetProperty("status").GetString() ?? "available";
                    var status = Enum.TryParse<EAvailabilitySlotStatus>(statusStr, true, out var parsedStatus) ? parsedStatus : EAvailabilitySlotStatus.Available;

                    context.AvailabilitySlots.Add(new AvailabilitySlot(publicId, doctorId, branchId, date, startTime, endTime, status));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Availability Slots successfully.");
            }

            // Seed MedicalRecords
            if (!context.MedicalRecords.Any() && root.TryGetProperty("medicalRecords", out var mrProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Medical Records...");
                foreach (var item in mrProp.EnumerateArray())
                {
                    var code = item.GetProperty("code").GetString() ?? "";
                    var patientId = item.GetProperty("id_patient").GetString() ?? "";
                    var appointmentId = item.GetProperty("id_appointment").GetString() ?? "";

                    var mr = new MedicalRecord(appointmentId, patientId);
                    
                    // Set Code using reflection to match db.json exact values
                    var codeProp = typeof(MedicalRecord).GetProperty("Code");
                    codeProp?.SetValue(mr, code);

                    context.MedicalRecords.Add(mr);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Medical Records successfully.");
            }

            // Seed Diagnoses
            if (!context.Diagnoses.Any() && root.TryGetProperty("diagnoses", out var diagnosesProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Diagnoses...");
                foreach (var item in diagnosesProp.EnumerateArray())
                {
                    var mrId = item.GetProperty("id_medical_record").GetString() ?? "";
                    var description = item.GetProperty("description").GetString() ?? "";

                    context.Diagnoses.Add(new Diagnosis(mrId, description));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Diagnoses successfully.");
            }

            // Seed Treatments
            if (!context.Treatments.Any() && root.TryGetProperty("treatments", out var treatmentsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Treatments...");
                foreach (var item in treatmentsProp.EnumerateArray())
                {
                    var mrId = item.GetProperty("id_medical_record").GetString() ?? "";
                    var description = item.GetProperty("description").GetString() ?? "";

                    context.Treatments.Add(new Treatment(mrId, description));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Treatments successfully.");
            }

            // Seed Prescriptions
            var prescriptionIdMap = new Dictionary<string, int>();
            if (!context.Prescriptions.Any() && root.TryGetProperty("prescriptions", out var prescriptionsProp))
            {
                Console.WriteLine("[DbSeeder] Seeding Prescriptions...");
                foreach (var item in prescriptionsProp.EnumerateArray())
                {
                    var mockId = item.GetProperty("id").GetString() ?? "";
                    var mrId = item.GetProperty("id_medical_record").GetString() ?? "";

                    var prescription = new Prescription(mrId);
                    context.Prescriptions.Add(prescription);
                    
                    // We will save changes to generate IDs and map them
                    await context.SaveChangesAsync();
                    prescriptionIdMap[mockId] = prescription.Id;
                }
                Console.WriteLine("[DbSeeder] Seeded Prescriptions successfully.");
            }

            // Seed PrescriptionDetails
            if (!context.PrescriptionDetails.Any() && root.TryGetProperty("prescriptionDetails", out var pdProp) && prescriptionIdMap.Count > 0)
            {
                Console.WriteLine("[DbSeeder] Seeding Prescription Details...");
                foreach (var item in pdProp.EnumerateArray())
                {
                    var mockPrescriptionId = item.GetProperty("id_prescription").GetString() ?? "";
                    if (!prescriptionIdMap.TryGetValue(mockPrescriptionId, out var prescriptionId))
                    {
                        continue; // Skip if we don't have the matched prescription
                    }

                    var mockMedicineIdName = item.GetProperty("id_medicine").GetString() ?? "";
                    var medicineName = item.GetProperty("medicine_name").GetString() ?? mockMedicineIdName;

                    // Try to find the seeded medicine in the DB to map its ID
                    var medicine = context.Medicines.Local.FirstOrDefault(m => m.Name.Equals(medicineName, StringComparison.OrdinalIgnoreCase))
                                   ?? context.Medicines.FirstOrDefault(m => m.Name.ToLower() == medicineName.ToLower());
                    
                    int? medicineId = medicine?.Id;

                    var frequency = item.GetProperty("frequency").GetString() ?? "";
                    var duration = item.GetProperty("duration").GetString() ?? "";

                    // Parse Dose
                    var doseAmount = 0;
                    if (item.TryGetProperty("dose", out var doseAmountProp))
                    {
                        if (doseAmountProp.ValueKind == JsonValueKind.Number)
                        {
                            doseAmount = doseAmountProp.GetInt32();
                        }
                        else if (doseAmountProp.ValueKind == JsonValueKind.String && int.TryParse(doseAmountProp.GetString(), out var parsedAmt))
                        {
                            doseAmount = parsedAmt;
                        }
                    }

                    var doseUnitStr = "mg";
                    if (item.TryGetProperty("dose_unit_type", out var doseUnitProp))
                    {
                        doseUnitStr = doseUnitProp.GetString() ?? "mg";
                    }

                    var doseUnit = Enum.TryParse<DoseUnitType>(doseUnitStr, true, out var parsedDoseUnit) ? parsedDoseUnit : DoseUnitType.Mg;
                    var dose = new Dose(doseAmount, doseUnit);

                    context.PrescriptionDetails.Add(new PrescriptionDetail(prescriptionId, medicineId, medicineName, dose, frequency, duration));
                }
                await context.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Seeded Prescription Details successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] ERROR: Failed to seed data from db.json. Details: {ex.Message}");
        }
    }
}
