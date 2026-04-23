using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260106pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeNumber",
                table: "EmployeeInfos",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "EmployeeChangeLogs",
                newName: "Objectid");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "EmployeeInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectName",
                table: "EmployeeChangeLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "EmployeeInfos");

            migrationBuilder.DropColumn(
                name: "ObjectName",
                table: "EmployeeChangeLogs");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "EmployeeInfos",
                newName: "EmployeeNumber");

            migrationBuilder.RenameColumn(
                name: "Objectid",
                table: "EmployeeChangeLogs",
                newName: "EmployeeId");

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "EmployeeInfos",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
