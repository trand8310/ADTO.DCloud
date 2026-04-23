using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260305pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptID",
                table: "Adto_WorkOverTime");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_WorkOverTime",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "Adto_ProductApplyDetail",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Adto_ProductApply",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_Out",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_Onb",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_OaPolicyApproval",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "Adto_MerchandiseInventoryApplyDetail",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Adto_MerchandiseInventoryApply",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_InformalPetition",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_FreeInformalPetition",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_Att",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Adto_Abs",
                newName: "Remark");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Adto_WorkOverTime",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Adto_WorkOverTime");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_WorkOverTime",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_ProductApplyDetail",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_ProductApply",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_Out",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_Onb",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_OaPolicyApproval",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_MerchandiseInventoryApplyDetail",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_MerchandiseInventoryApply",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_InformalPetition",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_FreeInformalPetition",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_Att",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "Adto_Abs",
                newName: "Reason");

            migrationBuilder.AddColumn<long>(
                name: "DeptID",
                table: "Adto_WorkOverTime",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
