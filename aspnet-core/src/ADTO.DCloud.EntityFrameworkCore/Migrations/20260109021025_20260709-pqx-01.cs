using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260709pqx01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeContracts_EmployeeInfos_EmployeeId",
                table: "EmployeeContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFamilies_EmployeeInfos_EmployeeId",
                table: "EmployeeFamilies");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeFamilies_EmployeeId",
                table: "EmployeeFamilies");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeContracts_EmployeeId",
                table: "EmployeeContracts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "EmployeeFamilies");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "EmployeeContracts");

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FamilieId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfos_ContractId",
                table: "EmployeeInfos",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfos_FamilieId",
                table: "EmployeeInfos",
                column: "FamilieId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_EmployeeContracts_ContractId",
                table: "EmployeeInfos",
                column: "ContractId",
                principalTable: "EmployeeContracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_EmployeeFamilies_FamilieId",
                table: "EmployeeInfos",
                column: "FamilieId",
                principalTable: "EmployeeFamilies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_EmployeeContracts_ContractId",
                table: "EmployeeInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_EmployeeFamilies_FamilieId",
                table: "EmployeeInfos");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInfos_ContractId",
                table: "EmployeeInfos");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInfos_FamilieId",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "FamilieId",
                table: "EmployeeInfos");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "EmployeeFamilies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "EmployeeContracts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFamilies_EmployeeId",
                table: "EmployeeFamilies",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_EmployeeId",
                table: "EmployeeContracts",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeContracts_EmployeeInfos_EmployeeId",
                table: "EmployeeContracts",
                column: "EmployeeId",
                principalTable: "EmployeeInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeFamilies_EmployeeInfos_EmployeeId",
                table: "EmployeeFamilies",
                column: "EmployeeId",
                principalTable: "EmployeeInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
