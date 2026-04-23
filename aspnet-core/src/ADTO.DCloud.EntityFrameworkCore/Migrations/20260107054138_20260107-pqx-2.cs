using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260107pqx2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceTimeRules_AttendanceLocations_LocationId",
                table: "AttendanceTimeRules");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTimeRules_LocationId",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "AMInTime",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "AMOutTime",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "PMInTime",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "PMOutTime",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "AttendanceTimeRules");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "AttendanceTimeRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "AttendanceTimeRules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleName",
                table: "AttendanceTimeRules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "RuleName",
                table: "AttendanceTimeRules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AMInTime",
                table: "AttendanceTimeRules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AMOutTime",
                table: "AttendanceTimeRules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AttendanceTimeRules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PMInTime",
                table: "AttendanceTimeRules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PMOutTime",
                table: "AttendanceTimeRules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "AttendanceTimeRules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTimeRules_LocationId",
                table: "AttendanceTimeRules",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceTimeRules_AttendanceLocations_LocationId",
                table: "AttendanceTimeRules",
                column: "LocationId",
                principalTable: "AttendanceLocations",
                principalColumn: "Id");
        }
    }
}
