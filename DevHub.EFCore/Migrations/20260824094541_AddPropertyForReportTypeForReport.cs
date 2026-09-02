using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyForReportTypeForReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reports");
        }
    }
}
