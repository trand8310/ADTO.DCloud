using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260407hj01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "CommodityStockCategory",
                type: "int",
                maxLength: 128,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CommodityStockCategory",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 128);
        }
    }
}
