using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosisCatalogByBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cie10_code",
                table: "diagnoses",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "diagnosis_catalog_source",
                table: "diagnoses",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "MINSA_CIE10");

            migrationBuilder.AddColumn<string>(
                name: "diagnosis_catalog_source",
                table: "branches",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "MINSA_CIE10");

            migrationBuilder.CreateTable(
                name: "diagnosis_catalog_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    source = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    search_text = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_diagnosis_catalog_entries", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_diagnosis_catalog_entries_code",
                table: "diagnosis_catalog_entries",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "i_x_diagnosis_catalog_entries_search_text",
                table: "diagnosis_catalog_entries",
                column: "search_text");

            migrationBuilder.CreateIndex(
                name: "i_x_diagnosis_catalog_entries_source_code",
                table: "diagnosis_catalog_entries",
                columns: new[] { "source", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "diagnosis_catalog_entries");

            migrationBuilder.DropColumn(
                name: "cie10_code",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "diagnosis_catalog_source",
                table: "diagnoses");

            migrationBuilder.DropColumn(
                name: "diagnosis_catalog_source",
                table: "branches");
        }
    }
}
