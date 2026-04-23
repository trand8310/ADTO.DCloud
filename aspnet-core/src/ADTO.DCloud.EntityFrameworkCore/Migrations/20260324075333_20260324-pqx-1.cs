using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260324pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceTimes_AttendanceLocations_LocationId",
                table: "AttendanceTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceTimes_AttendanceTimeRules_AttendanceTimeRuleId",
                table: "AttendanceTimes");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTimes_AttendanceTimeRuleId",
                table: "AttendanceTimes");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTimes_LocationId",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DisableUserWrokFlows");

            migrationBuilder.DropColumn(
                name: "AttendanceTimeRuleId",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "AttendanceTimes");

            migrationBuilder.AddColumn<DateTime>(
                name: "EDate",
                table: "AttendanceTimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AttendanceTimes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "AttendanceTimes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SDate",
                table: "AttendanceTimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AttendanceTimeIds",
                table: "AttendanceTimeRules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EDate",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "SDate",
                table: "AttendanceTimes");

            migrationBuilder.DropColumn(
                name: "AttendanceTimeIds",
                table: "AttendanceTimeRules");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "DisableUserWrokFlows",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "AttendanceTimeRuleId",
                table: "AttendanceTimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AttendanceTimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "AttendanceTimes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTimes_AttendanceTimeRuleId",
                table: "AttendanceTimes",
                column: "AttendanceTimeRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTimes_LocationId",
                table: "AttendanceTimes",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceTimes_AttendanceLocations_LocationId",
                table: "AttendanceTimes",
                column: "LocationId",
                principalTable: "AttendanceLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceTimes_AttendanceTimeRules_AttendanceTimeRuleId",
                table: "AttendanceTimes",
                column: "AttendanceTimeRuleId",
                principalTable: "AttendanceTimeRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
