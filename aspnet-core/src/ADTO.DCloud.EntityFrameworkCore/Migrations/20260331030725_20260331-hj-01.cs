using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260331hj01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConvertType",
                table: "ExcelImportFields",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DictTypeCode",
                table: "ExcelImportFields",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchField",
                table: "ExcelImportFields",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelationEntity",
                table: "ExcelImportFields",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertType",
                table: "ExcelImportFields");

            migrationBuilder.DropColumn(
                name: "DictTypeCode",
                table: "ExcelImportFields");

            migrationBuilder.DropColumn(
                name: "MatchField",
                table: "ExcelImportFields");

            migrationBuilder.DropColumn(
                name: "RelationEntity",
                table: "ExcelImportFields");
        }
    }
}
