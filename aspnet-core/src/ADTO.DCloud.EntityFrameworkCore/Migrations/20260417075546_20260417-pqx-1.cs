using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260417pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "temp_CHECKINOUT",
                columns: table => new
                {
                    USERID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SENSORID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHECKTYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VERIFYCODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Memoinfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserExtFmt = table.Column<short>(type: "smallint", nullable: false),
                    mask_flag = table.Column<int>(type: "int", nullable: false),
                    temperature = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temp_CHECKINOUT", x => x.USERID);
                });

            migrationBuilder.CreateTable(
                name: "temp_USERINFO",
                columns: table => new
                {
                    USERID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BADGENUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temp_USERINFO", x => x.USERID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "temp_CHECKINOUT");

            migrationBuilder.DropTable(
                name: "temp_USERINFO");
        }
    }
}
