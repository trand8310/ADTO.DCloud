using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260108pqx3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos");

            migrationBuilder.RenameColumn(
                name: "ManagerIdId",
                table: "EmployeeInfos",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos",
                newName: "IX_EmployeeInfos_ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerId",
                table: "EmployeeInfos",
                column: "ManagerId",
                principalTable: "EmployeeInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerId",
                table: "EmployeeInfos");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "EmployeeInfos",
                newName: "ManagerIdId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeInfos_ManagerId",
                table: "EmployeeInfos",
                newName: "IX_EmployeeInfos_ManagerIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerIdId",
                table: "EmployeeInfos",
                column: "ManagerIdId",
                principalTable: "EmployeeInfos",
                principalColumn: "Id");
        }
    }
}
