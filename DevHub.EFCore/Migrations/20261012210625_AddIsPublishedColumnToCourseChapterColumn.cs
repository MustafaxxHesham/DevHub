using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedColumnToCourseChapterColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Courses_CourseId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Users_UserId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTrack_UserCourse_UserCourseId",
                table: "UserCourseTrack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourse",
                table: "UserCourse");

            migrationBuilder.RenameTable(
                name: "UserCourse",
                newName: "UsersCourses");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_UserId",
                table: "UsersCourses",
                newName: "IX_UsersCourses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_CourseId",
                table: "UsersCourses",
                newName: "IX_UsersCourses_CourseId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Feedbacks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "CourseChapters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersCourses",
                table: "UsersCourses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTrack_UsersCourses_UserCourseId",
                table: "UserCourseTrack",
                column: "UserCourseId",
                principalTable: "UsersCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCourses_Courses_CourseId",
                table: "UsersCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCourses_Users_UserId",
                table: "UsersCourses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTrack_UsersCourses_UserCourseId",
                table: "UserCourseTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersCourses_Courses_CourseId",
                table: "UsersCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersCourses_Users_UserId",
                table: "UsersCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersCourses",
                table: "UsersCourses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "CourseChapters");

            migrationBuilder.RenameTable(
                name: "UsersCourses",
                newName: "UserCourse");

            migrationBuilder.RenameIndex(
                name: "IX_UsersCourses_UserId",
                table: "UserCourse",
                newName: "IX_UserCourse_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersCourses_CourseId",
                table: "UserCourse",
                newName: "IX_UserCourse_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourse",
                table: "UserCourse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Courses_CourseId",
                table: "UserCourse",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Users_UserId",
                table: "UserCourse",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTrack_UserCourse_UserCourseId",
                table: "UserCourseTrack",
                column: "UserCourseId",
                principalTable: "UserCourse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
