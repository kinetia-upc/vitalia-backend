using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointment_fees",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
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
                name: "branches",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    healthcare_center_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    address_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
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
                name: "healthcare_centers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    alliance_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    alliance_finish_date = table.Column<DateTime>(type: "date", nullable: true),
                    ruc_number = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_healthcare_centers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_appointment_fees_branch_id",
                table: "appointment_fees",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "i_x_appointment_fees_public_id",
                table: "appointment_fees",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_branches_healthcare_center_id",
                table: "branches",
                column: "healthcare_center_id");

            migrationBuilder.CreateIndex(
                name: "i_x_branches_public_id",
                table: "branches",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_healthcare_centers_public_id",
                table: "healthcare_centers",
                column: "public_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_fees");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropTable(
                name: "healthcare_centers");
        }
    }
}
