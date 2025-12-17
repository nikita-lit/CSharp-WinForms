using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinForms.Migrations
{
    /// <inheritdoc />
    public partial class CarsService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CarServices",
                table: "CarServices");

            migrationBuilder.RenameColumn(
                name: "DateOfService",
                table: "CarServices",
                newName: "EndTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "CarServices",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarServices",
                table: "CarServices",
                columns: new[] { "CarId", "ServiceId", "StartTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CarServices",
                table: "CarServices");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "CarServices");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "CarServices",
                newName: "DateOfService");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarServices",
                table: "CarServices",
                columns: new[] { "CarId", "ServiceId", "DateOfService" });
        }
    }
}
