using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260312pqx3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adto_StockApplicationItems_Adto_ProductApply_ProductAppId",
                table: "Adto_StockApplicationItems");

            migrationBuilder.DropTable(
                name: "Adto_ProductApply");

            migrationBuilder.RenameColumn(
                name: "ProductAppId",
                table: "Adto_StockApplicationItems",
                newName: "StockApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Adto_StockApplicationItems_ProductAppId",
                table: "Adto_StockApplicationItems",
                newName: "IX_Adto_StockApplicationItems_StockApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adto_StockApplicationItems_Adto_StockApplications_StockApplicationId",
                table: "Adto_StockApplicationItems",
                column: "StockApplicationId",
                principalTable: "Adto_StockApplications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adto_StockApplicationItems_Adto_StockApplications_StockApplicationId",
                table: "Adto_StockApplicationItems");

            migrationBuilder.RenameColumn(
                name: "StockApplicationId",
                table: "Adto_StockApplicationItems",
                newName: "ProductAppId");

            migrationBuilder.RenameIndex(
                name: "IX_Adto_StockApplicationItems_StockApplicationId",
                table: "Adto_StockApplicationItems",
                newName: "IX_Adto_StockApplicationItems_ProductAppId");

            migrationBuilder.CreateTable(
                name: "Adto_ProductApply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsInformatioMannager = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adto_ProductApply", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Adto_StockApplicationItems_Adto_ProductApply_ProductAppId",
                table: "Adto_StockApplicationItems",
                column: "ProductAppId",
                principalTable: "Adto_ProductApply",
                principalColumn: "Id");
        }
    }
}
