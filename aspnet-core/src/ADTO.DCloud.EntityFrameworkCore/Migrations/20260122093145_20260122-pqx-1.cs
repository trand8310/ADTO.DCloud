using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADTO.DCloud.Migrations
{
    /// <inheritdoc />
    public partial class _20260122pqx1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_CompanyId",
                table: "EmployeeInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Adto_Abs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Days = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MarriageAttach = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RestSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Adto_Abs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_EmailRequireForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    EmailName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailPwd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_Adto_EmailRequireForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_FreeInformalPetition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Adto_FreeInformalPetition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_InformalPetition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    ContractName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContractAmount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Commission = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContractNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Files = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Adto_InformalPetition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_MerchandiseInventoryApply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Adto_MerchandiseInventoryApply", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_OaPolicyApproval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    ApplyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    IsUnionApproval = table.Column<int>(type: "int", maxLength: 500, nullable: false),
                    UnionAttachment = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    PublishAttachment = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    CollegeAttachment = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovalIssuer = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
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
                    table.PrimaryKey("PK_Adto_OaPolicyApproval", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_ProductApply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsInformatioMannager = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Adto_ProductApply", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_TrainingRequireForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_Adto_TrainingRequireForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_TrainingRoomRequireForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    SDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Participants = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TrainingRoomName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Adto_TrainingRoomRequireForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_UseCar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    SDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SMileage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EMileage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PassengerNum = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarNum = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ApplyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    States = table.Column<int>(type: "int", nullable: true),
                    Route = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Driver = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EstimateReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SImage = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EImage = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_Adto_UseCar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_WorkOverTime",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeptID = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Times = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_Adto_WorkOverTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AMInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AMOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AMInAttTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AMOutAttTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AMInArea = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AMOutArea = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PMInTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMOutTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMInAttTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMOutAttTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PMInArea = table.Column<int>(type: "int", nullable: true),
                    PMOutArea = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AMInType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AMOutType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PMInType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PMOutType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WorkArea = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendancerMealStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LunchCount = table.Column<int>(type: "int", nullable: false),
                    LunchPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DinnerCount = table.Column<int>(type: "int", nullable: false),
                    DinnerPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendancerMealStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommodityStockCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SortCode = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
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
                    table.PrimaryKey("PK_CommodityStockCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DingdingUserAttLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    DetailPlace = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    ImageLists = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Place = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsNormal = table.Column<int>(type: "int", nullable: true),
                    IsDefaultAddr = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DingdingUserAttLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisableUserWrokFlows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceTable = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisableUserWrokFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullHead = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullHeadColor = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BriefHead = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CompileName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TagWord = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    SourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NewsContent = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    ReleaseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PV = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsViewLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsViewLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OA4PQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionsTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EnabledMark = table.Column<int>(type: "int", nullable: true),
                    SortCode = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Option_1 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Option_2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Option_3 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Option_4 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                    table.PrimaryKey("PK_OA4PQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OA4PStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrueName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PostName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Option_1_Total = table.Column<int>(type: "int", nullable: true),
                    Option_2_Total = table.Column<int>(type: "int", nullable: true),
                    Option_3_Total = table.Column<int>(type: "int", nullable: true),
                    Option_4_Total = table.Column<int>(type: "int", nullable: true),
                    Option_Sum_Total = table.Column<int>(type: "int", nullable: true),
                    UesTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Inviter = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OA4PStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicHoliDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoliDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IsMorningMeetingTime = table.Column<int>(type: "int", nullable: false),
                    IsLegalHoliday = table.Column<int>(type: "int", nullable: false),
                    IsSpecial = table.Column<int>(type: "int", nullable: true),
                    IsSixDay = table.Column<int>(type: "int", nullable: true),
                    SizeWeek = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicHoliDay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adto_MerchandiseInventoryApplyDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchandiseInventoryApplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApplyType = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Adto_MerchandiseInventoryApplyDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adto_MerchandiseInventoryApplyDetail_Adto_MerchandiseInventoryApply_MerchandiseInventoryApplyId",
                        column: x => x.MerchandiseInventoryApplyId,
                        principalTable: "Adto_MerchandiseInventoryApply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Adto_ProductApplyDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductAppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ApplyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Adto_ProductApplyDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adto_ProductApplyDetail_Adto_ProductApply_ProductAppId",
                        column: x => x.ProductAppId,
                        principalTable: "Adto_ProductApply",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommodityStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SN = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Preice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SortCode = table.Column<int>(type: "int", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
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
                    table.PrimaryKey("PK_CommodityStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommodityStocks_CommodityStockCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CommodityStockCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommodityStocksRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommodityStockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Preice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: true),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_CommodityStocksRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommodityStocksRecords_CommodityStocks_CommodityStockId",
                        column: x => x.CommodityStockId,
                        principalTable: "CommodityStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adto_MerchandiseInventoryApplyDetail_MerchandiseInventoryApplyId",
                table: "Adto_MerchandiseInventoryApplyDetail",
                column: "MerchandiseInventoryApplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Adto_ProductApplyDetail_ProductAppId",
                table: "Adto_ProductApplyDetail",
                column: "ProductAppId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityStocks_CategoryId",
                table: "CommodityStocks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityStocksRecords_CommodityStockId",
                table: "CommodityStocksRecords",
                column: "CommodityStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_CompanyId",
                table: "EmployeeInfos",
                column: "CompanyId",
                principalTable: "OrganizationUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_CompanyId",
                table: "EmployeeInfos");

            migrationBuilder.DropTable(
                name: "Adto_Abs");

            migrationBuilder.DropTable(
                name: "Adto_EmailRequireForm");

            migrationBuilder.DropTable(
                name: "Adto_FreeInformalPetition");

            migrationBuilder.DropTable(
                name: "Adto_InformalPetition");

            migrationBuilder.DropTable(
                name: "Adto_MerchandiseInventoryApplyDetail");

            migrationBuilder.DropTable(
                name: "Adto_OaPolicyApproval");

            migrationBuilder.DropTable(
                name: "Adto_ProductApplyDetail");

            migrationBuilder.DropTable(
                name: "Adto_TrainingRequireForm");

            migrationBuilder.DropTable(
                name: "Adto_TrainingRoomRequireForm");

            migrationBuilder.DropTable(
                name: "Adto_UseCar");

            migrationBuilder.DropTable(
                name: "Adto_WorkOverTime");

            migrationBuilder.DropTable(
                name: "AttendanceLogs");

            migrationBuilder.DropTable(
                name: "AttendancerMealStatistics");

            migrationBuilder.DropTable(
                name: "CommodityStocksRecords");

            migrationBuilder.DropTable(
                name: "DingdingUserAttLogs");

            migrationBuilder.DropTable(
                name: "DisableUserWrokFlows");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "NewsViewLogs");

            migrationBuilder.DropTable(
                name: "OA4PQuestion");

            migrationBuilder.DropTable(
                name: "OA4PStatistics");

            migrationBuilder.DropTable(
                name: "PublicHoliDay");

            migrationBuilder.DropTable(
                name: "Adto_MerchandiseInventoryApply");

            migrationBuilder.DropTable(
                name: "Adto_ProductApply");

            migrationBuilder.DropTable(
                name: "CommodityStocks");

            migrationBuilder.DropTable(
                name: "CommodityStockCategory");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "EmployeeInfos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInfos_OrganizationUnits_CompanyId",
                table: "EmployeeInfos",
                column: "CompanyId",
                principalTable: "OrganizationUnits",
                principalColumn: "Id");
        }
    }
}
