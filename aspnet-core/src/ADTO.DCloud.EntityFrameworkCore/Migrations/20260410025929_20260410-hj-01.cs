using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260410hj01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Preice",
                table: "CommodityStocksRecords",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Preice",
                table: "CommodityStocks",
                newName: "Price");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "CommodityStocks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "CommodityStocksRecords",
                newName: "Preice");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "CommodityStocks",
                newName: "Preice");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "CommodityStocks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
