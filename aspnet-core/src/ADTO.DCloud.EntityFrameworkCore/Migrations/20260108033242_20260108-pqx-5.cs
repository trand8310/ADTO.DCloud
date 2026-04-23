using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260108pqx5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerId",
                table: "EmployeeInfos");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfos_UserId",
                table: "EmployeeInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_Users_ManagerId",
                table: "EmployeeInfos",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_Users_UserId",
                table: "EmployeeInfos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_Users_ManagerId",
                table: "EmployeeInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_Users_UserId",
                table: "EmployeeInfos");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInfos_UserId",
                table: "EmployeeInfos");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_EmployeeInfos_ManagerId",
                table: "EmployeeInfos",
                column: "ManagerId",
                principalTable: "EmployeeInfos",
                principalColumn: "Id");
        }
    }
}
