using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260307hj01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodeRecords_CodeRecords_CodeRuleId",
                table: "CodeRecords");

            migrationBuilder.DropIndex(
                name: "IX_CodeRecords_CodeRuleId",
                table: "CodeRecords");

            migrationBuilder.DropColumn(
                name: "CodeRuleId",
                table: "CodeRecords");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "CodeRecords",
                newName: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeRecords_RuleId",
                table: "CodeRecords",
                column: "RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeRecords_CodeRules_RuleId",
                table: "CodeRecords",
                column: "RuleId",
                principalTable: "CodeRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodeRecords_CodeRules_RuleId",
                table: "CodeRecords");

            migrationBuilder.DropIndex(
                name: "IX_CodeRecords_RuleId",
                table: "CodeRecords");

            migrationBuilder.RenameColumn(
                name: "RuleId",
                table: "CodeRecords",
                newName: "RoleId");

            migrationBuilder.AddColumn<Guid>(
                name: "CodeRuleId",
                table: "CodeRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodeRecords_CodeRuleId",
                table: "CodeRecords",
                column: "CodeRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeRecords_CodeRecords_CodeRuleId",
                table: "CodeRecords",
                column: "CodeRuleId",
                principalTable: "CodeRecords",
                principalColumn: "Id");
        }
    }
}
