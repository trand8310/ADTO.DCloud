using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260312pqx2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adto_MerchandiseInventoryApplyDetail");

            migrationBuilder.DropTable(
                name: "Adto_ProductApplyDetail");

            migrationBuilder.DropTable(
                name: "Adto_MerchandiseInventoryApply");

            migrationBuilder.CreateTable(
                name: "Adto_StockApplicationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductAppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ApplyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BackUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BackUserId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 128, nullable: false),
                    BackRemark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReturnStatus = table.Column<bool>(type: "bit", nullable: false),
                    ApplyCount = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Adto_StockApplicationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adto_StockApplicationItems_Adto_ProductApply_ProductAppId",
                        column: x => x.ProductAppId,
                        principalTable: "Adto_ProductApply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Adto_StockApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Adto_StockApplications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adto_StockApplicationItems_ProductAppId",
                table: "Adto_StockApplicationItems",
                column: "ProductAppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adto_StockApplicationItems");

            migrationBuilder.DropTable(
                name: "Adto_StockApplications");

            migrationBuilder.CreateTable(
                name: "Adto_MerchandiseInventoryApply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adto_MerchandiseInventoryApply", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_ProductApplyDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductAppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplyCount = table.Column<int>(type: "int", nullable: false),
                    ApplyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BackRemark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BackUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BackUserId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 128, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReturnStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adto_ProductApplyDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adto_ProductApplyDetail_Adto_ProductApply_ProductAppId",
                        column: x => x.ProductAppId,
                        principalTable: "Adto_ProductApply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Adto_MerchandiseInventoryApplyDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchandiseInventoryApplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplyCount = table.Column<int>(type: "int", nullable: false),
                    ApplyType = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adto_MerchandiseInventoryApplyDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adto_MerchandiseInventoryApplyDetail_Adto_MerchandiseInventoryApply_MerchandiseInventoryApplyId",
                        column: x => x.MerchandiseInventoryApplyId,
                        principalTable: "Adto_MerchandiseInventoryApply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adto_MerchandiseInventoryApplyDetail_MerchandiseInventoryApplyId",
                table: "Adto_MerchandiseInventoryApplyDetail",
                column: "MerchandiseInventoryApplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Adto_ProductApplyDetail_ProductAppId",
                table: "Adto_ProductApplyDetail",
                column: "ProductAppId");
        }
    }
}
