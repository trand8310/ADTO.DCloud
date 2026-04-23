using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260108pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "EmployeeInfos");

            migrationBuilder.AlterColumn<string>(
                name: "PostLevelId",
                table: "EmployeeInfos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IsAttType",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeType",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckInRules",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfos_CompanyId",
                table: "EmployeeInfos",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfos_DepartmentId",
                table: "EmployeeInfos",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_CompanyId",
                table: "EmployeeInfos",
                column: "CompanyId",
                principalTable: "OrganizationUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_DepartmentId",
                table: "EmployeeInfos",
                column: "DepartmentId",
                principalTable: "OrganizationUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_CompanyId",
                table: "EmployeeInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_DepartmentId",
                table: "EmployeeInfos");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInfos_CompanyId",
                table: "EmployeeInfos");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInfos_DepartmentId",
                table: "EmployeeInfos");

            migrationBuilder.AlterColumn<int>(
                name: "PostLevelId",
                table: "EmployeeInfos",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsAttType",
                table: "EmployeeInfos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeType",
                table: "EmployeeInfos",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CheckInRules",
                table: "EmployeeInfos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
