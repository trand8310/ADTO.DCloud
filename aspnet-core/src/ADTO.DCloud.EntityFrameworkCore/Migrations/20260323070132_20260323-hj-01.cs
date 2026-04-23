using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260323hj01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "F_ErrorType",
                table: "ExcelImport",
                newName: "ErrorType");

            migrationBuilder.CreateTable(
                name: "ApplicationFormCheckDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAudit = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFormCheckDates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationFormCheckDates");

            migrationBuilder.RenameColumn(
                name: "ErrorType",
                table: "ExcelImport",
                newName: "F_ErrorType");
        }
    }
}
