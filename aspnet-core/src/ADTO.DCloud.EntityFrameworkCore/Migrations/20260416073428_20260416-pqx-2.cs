using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260416pqx2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineAddress = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    MachineIP = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    MachineNumber = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceMachines_AttendanceLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "AttendanceLocations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceMachines_LocationId",
                table: "AttendanceMachines",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceMachines");
        }
    }
}
