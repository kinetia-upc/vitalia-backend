using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedulingRosterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scheduling_branches",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_scheduling_branches", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scheduling_doctors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    id_user = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    specialty = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    branch_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_scheduling_doctors", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scheduling_patients",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    id_user = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    insurance_provider = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_scheduling_patients", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_availability_slots_doctor_id_branch_id_date_start_time",
                table: "availability_slots",
                columns: new[] { "doctor_id", "branch_id", "date", "start_time" });

            migrationBuilder.CreateIndex(
                name: "i_x_availability_slots_public_id",
                table: "availability_slots",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_appointments_doctor_id_scheduled_at",
                table: "appointments",
                columns: new[] { "doctor_id", "scheduled_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_appointments_public_id",
                table: "appointments",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_scheduling_branches_public_id",
                table: "scheduling_branches",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_scheduling_doctors_public_id",
                table: "scheduling_doctors",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_scheduling_patients_public_id",
                table: "scheduling_patients",
                column: "public_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scheduling_branches");

            migrationBuilder.DropTable(
                name: "scheduling_doctors");

            migrationBuilder.DropTable(
                name: "scheduling_patients");

            migrationBuilder.DropIndex(
                name: "i_x_availability_slots_doctor_id_branch_id_date_start_time",
                table: "availability_slots");

            migrationBuilder.DropIndex(
                name: "i_x_availability_slots_public_id",
                table: "availability_slots");

            migrationBuilder.DropIndex(
                name: "i_x_appointments_doctor_id_scheduled_at",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "i_x_appointments_public_id",
                table: "appointments");
        }
    }
}
