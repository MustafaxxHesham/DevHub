using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnWalletToWalletIdInUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Wallet",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuedAt",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedAt",
                table: "Wallets");

            migrationBuilder.AddColumn<Guid>(
                name: "Wallet",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
