using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260107pqx3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceTimeRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AMInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AMOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceTimes_AttendanceLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "AttendanceLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceTimes_AttendanceTimeRules_AttendanceTimeRuleId",
                        column: x => x.AttendanceTimeRuleId,
                        principalTable: "AttendanceTimeRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTimes_AttendanceTimeRuleId",
                table: "AttendanceTimes",
                column: "AttendanceTimeRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTimes_LocationId",
                table: "AttendanceTimes",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceTimes");
        }
    }
}
