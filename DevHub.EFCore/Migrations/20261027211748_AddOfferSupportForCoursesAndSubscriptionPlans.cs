using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddOfferSupportForCoursesAndSubscriptionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTrack_CourseChapters_ChapterId",
                table: "UserCourseTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTrack_CourseVideos_VideoId",
                table: "UserCourseTrack");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourseTrack",
                table: "UserCourseTrack");

            migrationBuilder.RenameTable(
                name: "UsersCourses",
                newName: "UserCourses");

            migrationBuilder.RenameTable(
                name: "UserCourseTrack",
                newName: "UserCourseTracks");

            migrationBuilder.RenameIndex(
                name: "IX_UsersCourses_UserId",
                table: "UserCourses",
                newName: "IX_UserCourses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsersCourses_CourseId",
                table: "UserCourses",
                newName: "IX_UserCourses_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseTrack_VideoId",
                table: "UserCourseTracks",
                newName: "IX_UserCourseTracks_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseTrack_UserCourseId",
                table: "UserCourseTracks",
                newName: "IX_UserCourseTracks_UserCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseTrack_ChapterId",
                table: "UserCourseTracks",
                newName: "IX_UserCourseTracks_ChapterId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSignIn",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourses",
                table: "UserCourses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourseTracks",
                table: "UserCourseTracks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountValue = table.Column<double>(type: "float", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offer_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseOffer",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    OffersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseOffer", x => new { x.CoursesId, x.OffersId });
                    table.ForeignKey(
                        name: "FK_CourseOffer_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CourseOffer_Offer_OffersId",
                        column: x => x.OffersId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "OfferSubscriptionPlan",
                columns: table => new
                {
                    OffersId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPlansId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferSubscriptionPlan", x => new { x.OffersId, x.SubscriptionPlansId });
                    table.ForeignKey(
                        name: "FK_OfferSubscriptionPlan_Offer_OffersId",
                        column: x => x.OffersId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_OfferSubscriptionPlan_SubscriptionPlans_SubscriptionPlansId",
                        column: x => x.SubscriptionPlansId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseOffer_OffersId",
                table: "CourseOffer",
                column: "OffersId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_UserId",
                table: "Offer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferSubscriptionPlan_SubscriptionPlansId",
                table: "OfferSubscriptionPlan",
                column: "SubscriptionPlansId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourses_Courses_CourseId",
                table: "UserCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourses_Users_UserId",
                table: "UserCourses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTracks_CourseChapters_ChapterId",
                table: "UserCourseTracks",
                column: "ChapterId",
                principalTable: "CourseChapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTracks_CourseVideos_VideoId",
                table: "UserCourseTracks",
                column: "VideoId",
                principalTable: "CourseVideos",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTracks_UserCourses_UserCourseId",
                table: "UserCourseTracks",
                column: "UserCourseId",
                principalTable: "UserCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCourses_Courses_CourseId",
                table: "UserCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourses_Users_UserId",
                table: "UserCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTracks_CourseChapters_ChapterId",
                table: "UserCourseTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTracks_CourseVideos_VideoId",
                table: "UserCourseTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseTracks_UserCourses_UserCourseId",
                table: "UserCourseTracks");

            migrationBuilder.DropTable(
                name: "CourseOffer");

            migrationBuilder.DropTable(
                name: "OfferSubscriptionPlan");

            migrationBuilder.DropTable(
                name: "Offer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourseTracks",
                table: "UserCourseTracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourses",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "LastSignIn",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UserCourseTracks",
                newName: "UserCourseTrack");

            migrationBuilder.RenameTable(
                name: "UserCourses",
                newName: "UsersCourses");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseTracks_VideoId",
                table: "UserCourseTrack",
                newName: "IX_UserCourseTrack_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseTracks_UserCourseId",
                table: "UserCourseTrack",
                newName: "IX_UserCourseTrack_UserCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseTracks_ChapterId",
                table: "UserCourseTrack",
                newName: "IX_UserCourseTrack_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourses_UserId",
                table: "UsersCourses",
                newName: "IX_UsersCourses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourses_CourseId",
                table: "UsersCourses",
                newName: "IX_UsersCourses_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourseTrack",
                table: "UserCourseTrack",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersCourses",
                table: "UsersCourses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTrack_CourseChapters_ChapterId",
                table: "UserCourseTrack",
                column: "ChapterId",
                principalTable: "CourseChapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseTrack_CourseVideos_VideoId",
                table: "UserCourseTrack",
                column: "VideoId",
                principalTable: "CourseVideos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCourses_Users_UserId",
                table: "UsersCourses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
