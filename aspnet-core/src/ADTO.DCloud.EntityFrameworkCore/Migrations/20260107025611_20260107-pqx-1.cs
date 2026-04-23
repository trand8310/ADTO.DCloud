using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260107pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "AttendanceTimeRules",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "AttendanceTimeRules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
