using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appointment_fees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    branch_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    speciality_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_appointment_fees", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    doctor_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    patient_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    branch_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    payment_status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_appointments", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "availability_slots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    doctor_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    branch_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_availability_slots", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "billing_claims",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    appointment_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    insurance_provider = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    patient_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    provider_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    clinical_compliance = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    cycle_status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_billing_claims", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "branch_medicines",
                columns: table => new
                {
                    branch_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    medicine_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_branch_medicines", x => new { x.branch_id, x.medicine_id });
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    healthcare_center_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    address = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_branches", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "diagnoses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    medical_record_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_diagnoses", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "doctor_specialities",
                columns: table => new
                {
                    doctor_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    speciality_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_doctor_specialities", x => new { x.doctor_id, x.speciality_id });
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "doctors",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    license_number = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    cmp_number = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_doctors", x => x.user_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "healthcare_centers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    alliance_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    alliance_finish_date = table.Column<DateTime>(type: "date", nullable: true),
                    ruc_number = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_healthcare_centers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medical_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    patient_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    doctor_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    appointment_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    medical_record_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    type = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_medical_orders", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medical_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    appointment_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    patient_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_medical_records", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medicine_restocks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    branch_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    medicine_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_medicine_restocks", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "medicines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    unit_quantity = table.Column<int>(type: "int", nullable: false),
                    unit_type = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_medicines", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    insurance_provider = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    policy_number = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    active_thru = table.Column<DateTime>(type: "date", nullable: true),
                    emergency_contact_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    emergency_contact_phone = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_patients", x => x.user_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "prescription_details",
                columns: table => new
                {
                    prescription_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    medicine_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    frequency = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_prescription_details", x => new { x.prescription_id, x.medicine_id });
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "prescriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    medical_record_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_prescriptions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "specialities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_specialities", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    medical_record_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_treatments", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    healthcare_center_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    paternal_surname = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    maternal_surname = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    identity_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    identity_number = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    birth_date = table.Column<DateTime>(type: "date", nullable: false),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    phone = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    gender = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    address = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: false),
                    role = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_appointment_fees_branch_id",
                table: "appointment_fees",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "i_x_appointment_fees_code",
                table: "appointment_fees",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_appointments_code",
                table: "appointments",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_appointments_doctor_id_scheduled_at",
                table: "appointments",
                columns: new[] { "doctor_id", "scheduled_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_availability_slots_code",
                table: "availability_slots",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_availability_slots_doctor_id_branch_id_date_start_time",
                table: "availability_slots",
                columns: new[] { "doctor_id", "branch_id", "date", "start_time" });

            migrationBuilder.CreateIndex(
                name: "i_x_billing_claims_code",
                table: "billing_claims",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_branches_code",
                table: "branches",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_branches_healthcare_center_id",
                table: "branches",
                column: "healthcare_center_id");

            migrationBuilder.CreateIndex(
                name: "i_x_diagnoses_code",
                table: "diagnoses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_doctors_code",
                table: "doctors",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_healthcare_centers_code",
                table: "healthcare_centers",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_medical_orders_code",
                table: "medical_orders",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_medical_records_code",
                table: "medical_records",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_medical_records_patient_id_appointment_id",
                table: "medical_records",
                columns: new[] { "patient_id", "appointment_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_medicine_restocks_code",
                table: "medicine_restocks",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_medicines_code",
                table: "medicines",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_medicines_name_unit_quantity_unit_type",
                table: "medicines",
                columns: new[] { "name", "unit_quantity", "unit_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_patients_code",
                table: "patients",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_prescriptions_code",
                table: "prescriptions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_specialities_code",
                table: "specialities",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_treatments_code",
                table: "treatments",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_identity_number",
                table: "users",
                column: "identity_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_fees");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "availability_slots");

            migrationBuilder.DropTable(
                name: "billing_claims");

            migrationBuilder.DropTable(
                name: "branch_medicines");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropTable(
                name: "diagnoses");

            migrationBuilder.DropTable(
                name: "doctor_specialities");

            migrationBuilder.DropTable(
                name: "doctors");

            migrationBuilder.DropTable(
                name: "healthcare_centers");

            migrationBuilder.DropTable(
                name: "medical_orders");

            migrationBuilder.DropTable(
                name: "medical_records");

            migrationBuilder.DropTable(
                name: "medicine_restocks");

            migrationBuilder.DropTable(
                name: "medicines");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "prescription_details");

            migrationBuilder.DropTable(
                name: "prescriptions");

            migrationBuilder.DropTable(
                name: "specialities");

            migrationBuilder.DropTable(
                name: "treatments");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
