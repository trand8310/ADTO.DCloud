using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260416pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PMInArea",
                table: "AttendanceLogs");

            migrationBuilder.RenameColumn(
                name: "WorkArea",
                table: "AttendanceLogs",
                newName: "PMOutLocationId");

            migrationBuilder.RenameColumn(
                name: "PMOutArea",
                table: "AttendanceLogs",
                newName: "PMInLocationId");

            migrationBuilder.RenameColumn(
                name: "AMOutArea",
                table: "AttendanceLogs",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "AMInArea",
                table: "AttendanceLogs",
                newName: "AMOutLocationId");

            migrationBuilder.AddColumn<Guid>(
                name: "AMInLocationId",
                table: "AttendanceLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "Adto_OfficeSupplyApplications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AMInLocationId",
                table: "AttendanceLogs");

            migrationBuilder.RenameColumn(
                name: "PMOutLocationId",
                table: "AttendanceLogs",
                newName: "WorkArea");

            migrationBuilder.RenameColumn(
                name: "PMInLocationId",
                table: "AttendanceLogs",
                newName: "PMOutArea");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "AttendanceLogs",
                newName: "AMOutArea");

            migrationBuilder.RenameColumn(
                name: "AMOutLocationId",
                table: "AttendanceLogs",
                newName: "AMInArea");

            migrationBuilder.AddColumn<int>(
                name: "PMInArea",
                table: "AttendanceLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "Adto_OfficeSupplyApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
