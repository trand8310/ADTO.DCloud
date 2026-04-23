using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260420pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AttendanceTimeRules",
                type: "uniqueidentifier",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceTimeRules_AttendanceLocations_LocationId",
                table: "AttendanceTimeRules");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTimeRules_LocationId",
                table: "AttendanceTimeRules");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AttendanceTimeRules");
        }
    }
}
