using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260105pqx2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocationLatOrLon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AttendanceRadius = table.Column<long>(type: "bigint", nullable: true),
                    HasMealAllowance = table.Column<bool>(type: "bit", nullable: false),
                    MealAllowanceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MealAllowanceAmount = table.Column<double>(type: "float", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceTimeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AMInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AMOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceTimeRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeChangeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    PostLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IDCard = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    OICQ = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    WeChat = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    OfficeLocation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsAttType = table.Column<int>(type: "int", nullable: false),
                    IsAPPAtt = table.Column<int>(type: "int", nullable: false),
                    AttTimeRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InJobDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutJobDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeType = table.Column<int>(type: "int", nullable: true),
                    Postion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    BusinessGroup = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    CheckInRules = table.Column<int>(type: "int", nullable: false),
                    CheckInRulesEffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountApprovalStatus = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeContracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractExpirationDate = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContractSigningTimes = table.Column<int>(type: "int", nullable: true),
                    IsRegular = table.Column<int>(type: "int", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RegularDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangesStatus = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RecruitersId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 128, nullable: true),
                    Recruiters = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContractCompany = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContractCompanyId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContractDepartmentId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeContracts_EmployeeInfos_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeFamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoliticalOutlook = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Education = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Major = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NativePlace = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HomePhone = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsMarried = table.Column<int>(type: "int", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EmergencyContactMobile = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IdCardAttach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiplomaAttach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegreeDiplomaAttach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeavCertificateAttach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificatesAttach = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeFamilies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeFamilies_EmployeeInfos_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_EmployeeId",
                table: "EmployeeContracts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFamilies_EmployeeId",
                table: "EmployeeFamilies",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceLocations");

            migrationBuilder.DropTable(
                name: "AttendanceTimeRules");

            migrationBuilder.DropTable(
                name: "EmployeeChangeLogs");

            migrationBuilder.DropTable(
                name: "EmployeeContracts");

            migrationBuilder.DropTable(
                name: "EmployeeFamilies");

            migrationBuilder.DropTable(
                name: "EmployeeInfos");
        }
    }
}
