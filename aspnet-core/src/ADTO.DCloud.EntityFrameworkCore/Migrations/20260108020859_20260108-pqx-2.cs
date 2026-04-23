using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260108pqx2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagerIdId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos",
                column: "ManagerIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos",
                column: "ManagerIdId",
                principalTable: "EmployeeInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "ManagerIdId",
                table: "EmployeeInfos");
        }
    }
}
