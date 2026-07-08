using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalOrderPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "medical_orders",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priority",
                table: "medical_orders");
        }
    }
}
