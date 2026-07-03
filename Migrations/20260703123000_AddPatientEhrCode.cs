using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientEhrCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ehr_code",
                table: "patients",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.Sql(@"
                UPDATE patients
                SET ehr_code = CONCAT(
                    'EHR-',
                    LPAD(
                        CAST(SUBSTRING(code, LOCATE('-', code) + 1) AS UNSIGNED) + 10000,
                        5,
                        '0'
                    )
                )
                WHERE ehr_code IS NULL OR ehr_code = '';
            ");

            migrationBuilder.AlterColumn<string>(
                name: "ehr_code",
                table: "patients",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true)
                .Annotation("MySQL:Charset", "utf8mb4")
                .OldAnnotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_patients_e_h_r_code",
                table: "patients",
                column: "ehr_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_patients_e_h_r_code",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "ehr_code",
                table: "patients");
        }
    }
}
