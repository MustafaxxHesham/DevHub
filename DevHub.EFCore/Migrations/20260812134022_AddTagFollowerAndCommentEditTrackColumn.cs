using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddTagFollowerAndCommentEditTrackColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TagFollower",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFollower", x => new { x.TagId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TagFollower_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagFollower_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagFollower_UserId",
                table: "TagFollower",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagFollower");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Comments");
        }
    }
}
