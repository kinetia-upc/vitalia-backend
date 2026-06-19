using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "billing_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    claim_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "i_x_billing_claims_claim_code",
                table: "billing_claims",
                column: "claim_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "billing_claims");
        }
    }
}
