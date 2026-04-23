using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    
    /// <inheritdoc />
    public partial class _20260318hj01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatePerson",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "UpdatePerson",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "CreatePerson",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "UpdatePerson",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "CreatePerson",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "UpdatePerson",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "CreatePerson",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "UpdatePerson",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "CreatePerson",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "UpdatePerson",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Adto_StockApplications");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Base_Province",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Base_County",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Base_Country",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Base_City",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Base_Area",
                newName: "CreationTime");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "CountryId",
            //    table: "Base_Province",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "AreaId",
            //    table: "Base_Province",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Id",
            //    table: "Base_Province",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                table: "Base_Province",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterUserId",
                table: "Base_Province",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Base_Province",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Base_Province",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Base_Province",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Base_Province",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierUserId",
                table: "Base_Province",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Base_Province",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "ProvinceId",
            //    table: "Base_County",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "CountryId",
            //    table: "Base_County",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "CityId",
            //    table: "Base_County",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "AreaId",
            //    table: "Base_County",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Id",
            //    table: "Base_County",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                table: "Base_County",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterUserId",
                table: "Base_County",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Base_County",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Base_County",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Base_County",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Base_County",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierUserId",
                table: "Base_County",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Base_County",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "AreaId",
            //    table: "Base_Country",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Id",
            //    table: "Base_Country",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                table: "Base_Country",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterUserId",
                table: "Base_Country",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Base_Country",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Base_Country",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Base_Country",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Base_Country",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierUserId",
                table: "Base_Country",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Base_Country",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "ProvinceId",
            //    table: "Base_City",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "CountryId",
            //    table: "Base_City",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "AreaId",
            //    table: "Base_City",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Id",
            //    table: "Base_City",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                table: "Base_City",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterUserId",
                table: "Base_City",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Base_City",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Base_City",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Base_City",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Base_City",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierUserId",
                table: "Base_City",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Base_City",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "ParentId",
            //    table: "Base_Area",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Id",
            //    table: "Base_Area",
            //    type: "nvarchar(450)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                table: "Base_Area",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterUserId",
                table: "Base_Area",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Base_Area",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Base_Area",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Base_Area",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Base_Area",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierUserId",
                table: "Base_Area",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Base_Area",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Adto_OfficeSupplyApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Adto_OfficeSupplyApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_OfficeSupplyApplicationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeSupplyApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApplyType = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApplyCount = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_Adto_OfficeSupplyApplicationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adto_OfficeSupplyApplicationItems_Adto_OfficeSupplyApplications_OfficeSupplyApplicationId",
                        column: x => x.OfficeSupplyApplicationId,
                        principalTable: "Adto_OfficeSupplyApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adto_OfficeSupplyApplicationItems_OfficeSupplyApplicationId",
                table: "Adto_OfficeSupplyApplicationItems",
                column: "OfficeSupplyApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adto_OfficeSupplyApplicationItems");

            migrationBuilder.DropTable(
                name: "Adto_OfficeSupplyApplications");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Base_Province");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Base_County");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Base_Country");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Base_City");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Base_Area");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Base_Area");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Base_Province",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Base_County",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Base_Country",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Base_City",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Base_Area",
                newName: "UpdateTime");

            //migrationBuilder.AlterColumn<int>(
            //    name: "CountryId",
            //    table: "Base_Province",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "AreaId",
            //    table: "Base_Province",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Base_Province",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(450)")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreatePerson",
                table: "Base_Province",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Base_Province",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatePerson",
                table: "Base_Province",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "ProvinceId",
            //    table: "Base_County",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "CountryId",
            //    table: "Base_County",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "CityId",
            //    table: "Base_County",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "AreaId",
            //    table: "Base_County",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Base_County",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(450)")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreatePerson",
                table: "Base_County",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Base_County",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IsEnable",
                table: "Base_County",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatePerson",
                table: "Base_County",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Base_Country",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Base_Country",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(450)")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreatePerson",
                table: "Base_Country",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Base_Country",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatePerson",
                table: "Base_Country",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "ProvinceId",
            //    table: "Base_City",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "CountryId",
            //    table: "Base_City",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "AreaId",
            //    table: "Base_City",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Base_City",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(450)")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreatePerson",
                table: "Base_City",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Base_City",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IsEnable",
                table: "Base_City",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatePerson",
                table: "Base_City",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Base_Area",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Base_Area",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(450)")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CreatePerson",
                table: "Base_Area",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Base_Area",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IsEnable",
                table: "Base_Area",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatePerson",
                table: "Base_Area",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Adto_StockApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
