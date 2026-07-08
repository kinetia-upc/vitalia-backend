using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaliaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthcareCenterImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "healthcare_centers",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "healthcare_centers");
        }
    }
}
