using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class FollowersListSupportFollowerAndFollowedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollower_Users_FollowerUserId",
                table: "UserFollower");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollower",
                table: "UserFollower");

            migrationBuilder.RenameTable(
                name: "UserFollower",
                newName: "Followers");

            migrationBuilder.AddColumn<int>(
                name: "FollowedUserId",
                table: "Followers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followers",
                table: "Followers",
                columns: new[] { "FollowedUserId", "FollowerUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_Users_FollowedUserId",
                table: "Followers",
                column: "FollowedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_Users_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_Users_FollowedUserId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followers_Users_FollowerUserId",
                table: "Followers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Followers",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_FollowerUserId",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "FollowedUserId",
                table: "Followers");

            migrationBuilder.RenameTable(
                name: "Followers",
                newName: "UserFollower");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollower",
                table: "UserFollower",
                column: "FollowerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollower_Users_FollowerUserId",
                table: "UserFollower",
                column: "FollowerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
