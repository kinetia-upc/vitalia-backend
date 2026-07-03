using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AlignPatientEhrCodeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "i_x_patients_e_h_r_code",
                table: "patients",
                newName: "i_x_patients_ehr_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "i_x_patients_ehr_code",
                table: "patients",
                newName: "i_x_patients_e_h_r_code");
        }
    }
}
