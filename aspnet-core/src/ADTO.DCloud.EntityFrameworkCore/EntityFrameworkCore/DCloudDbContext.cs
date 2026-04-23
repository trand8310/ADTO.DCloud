using ADTO.DCloud.ApplicationForm;
using ADTO.DCloud.AreaBase;
using ADTO.DCloud.Attendances;
using ADTO.DCloud.Authorization.Delegation;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Chat;
using ADTO.DCloud.CodeTable;
using ADTO.DCloud.CommodityStocks;
using ADTO.DCloud.Customers;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DatabaseManager;
using ADTO.DCloud.DeptRoles;
using ADTO.DCloud.Editions;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.ExcelManager;
using ADTO.DCloud.Friendships;
using ADTO.DCloud.Media;
using ADTO.DCloud.Modules;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.MultiTenancy.Payments;
using ADTO.DCloud.News;
using ADTO.DCloud.OA4PTest;
using ADTO.DCloud.Project;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Surveys;
using ADTO.DCloud.Tasks;
using ADTO.DCloud.Training;
using ADTO.DCloud.WorkFlow.Delegate;
using ADTO.DCloud.WorkFlow.Processes;
using ADTO.DCloud.WorkFlow.Schemes;
using ADTO.DCloud.WorkFlow.StampManage;
using ADTO.DCloud.WorkFlow.Tasks;
using ADTO.OpenIddict.Applications;
using ADTO.OpenIddict.Authorizations;
using ADTO.OpenIddict.EntityFrameworkCore;
using ADTO.OpenIddict.Scopes;
using ADTO.OpenIddict.Tokens;
using ADTOSharp.Auditing;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Snowflakes;
using ADTOSharp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;





namespace ADTO.DCloud.EntityFrameworkCore;

public class DCloudDbContext : ADTOSharpZeroDbContext<Tenant, Role, User, DCloudDbContext>, IOpenIddictDbContext
{
    /* Define a DbSet for each entity of the application */
    /// <summary>
    /// 二进制文件
    /// </summary>
    public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

    /// <summary>
    /// 好友记录
    /// </summary>
    public virtual DbSet<Friendship> Friendships { get; set; }

    /// <summary>
    /// 消息记录
    /// </summary>
    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    /// <summary>
    /// 订阅的版本变更信息
    /// </summary>
    public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

    // public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

    /// <summary>
    /// 与用户关联的委托与代理
    /// </summary>
    public virtual DbSet<UserDelegation> UserDelegations { get; set; }
    /// <summary>
    /// 存储用户设置的密码数据
    /// </summary>
    public virtual DbSet<RecentPassword> RecentPasswords { get; set; }

    /// <summary>
    /// 功能模块
    /// </summary>
    public virtual DbSet<ADTO.DCloud.Modules.Module> Modules { get; set; }

    /// <summary>
    /// 功能模块元素
    /// </summary>
    public virtual DbSet<ModuleItem> ModuleItems { get; set; }

    /// <summary>
    /// 岗位管理
    /// </summary>
    public virtual DbSet<Post> Posts { get; set; }


    #region 流程
    /// <summary>
    /// 工作流模板
    /// </summary>
    public virtual DbSet<WorkFlowScheme> WorkFlowSchemes { get; set; }

    /// <summary>
    /// 工作流模板信息
    /// </summary>
    public virtual DbSet<WorkFlowSchemeinfo> WorkFlowSchemeinfos { get; set; }

    /// <summary>
    /// 工作流模板权限
    /// </summary>
    public virtual DbSet<WorkFlowSchemeauth> WorkFlowSchemeauths { get; set; }

    /// <summary>
    /// 工作流委托规则
    /// </summary>
    public virtual DbSet<WorkFlowDelegaterule> WorkFlowDelegaterules { get; set; }
    /// <summary>
    /// 工作流委托模板关系表
    /// </summary>
    public virtual DbSet<WorkFlowDelegateRelation> WorkFlowDelegateRelations { get; set; }

    /// <summary>
    /// 印章管理
    /// </summary>
    public virtual DbSet<WorkFlowStamp> WorkFlowStamps { get; set; }

    /// <summary>
    /// 工作流任务
    /// </summary>
    public virtual DbSet<WorkFlowTask> WorkFlowTasks { get; set; }

    /// <summary>
    /// 工作流任务日志
    /// </summary>
    public virtual DbSet<WorkFlowTaskLog> WorkFlowTaskLogs { get; set; }

    /// <summary>
    /// 工作流任务消息
    /// </summary>
    public virtual DbSet<WorkFlowTaskMsg> WorkFlowTaskMsgs { get; set; }

    /// <summary>
    /// 工作流任务执行人对应关系表
    /// </summary>
    //public virtual DbSet<WorkFlowTaskRelation> WorkFlowTaskRelations { get; set; }

    #endregion

    /// <summary>
    /// 工作流进程
    /// </summary>
    public virtual DbSet<WorkFlowProcess> WorkFlowProcesses { get; set; }

    /// <summary>
    /// 字典分类
    /// </summary>
    public virtual DbSet<DataItem.DataItem> DataItems { get; set; }
    /// <summary>
    /// 字典详情
    /// </summary>
    public virtual DbSet<DataItem.DataItemDetail> DataItemDetails { get; set; }

    /// <summary>
    /// 数据源视图
    /// </summary>
    public virtual DbSet<DataSource.DataSource> DataSources { get; set; }

    /// <summary>
    /// 表单模板信息（主表）
    /// </summary>
    public virtual DbSet<FormScheme.FormSchemeInfo> FormSchemeInfos { get; set; }

    /// <summary>
    /// 表单模板表
    /// </summary>
    public virtual DbSet<FormScheme.FormScheme> FormSchemes { get; set; }

    /// <summary>
    /// 用户关联岗位对象
    /// </summary>
    public virtual DbSet<UserPost> UserPosts { get; set; }

    /// <summary>
    /// (数据库表信息)表的实体
    /// </summary>
    public virtual DbSet<CodeTable.CodeTable> CodeTables { get; set; }

    /// <summary>
    /// (数据库表字段信息)表的实体
    /// </summary>
    public virtual DbSet<CodeColumns> CodeColumns { get; set; }

    /// <summary>
    /// 系统图标表
    /// </summary>
    public virtual DbSet<DataIcons.DataIcons> DataIcons { get; set; }

    /// <summary>
    /// 系统图标版本
    /// </summary>
    public virtual DbSet<DataIcons.DataIconver> DataIconvers { get; set; }

    /// <summary>
    /// 行政区域表
    /// </summary>
    public virtual DbSet<DataArea.DataArea> DataAreas { get; set; }

    /// <summary>
    /// 文件管理表
    /// </summary>
    public virtual DbSet<UploadFile> UploadFiles { get; set; }

    /// <summary>
    /// 所属文件夹类别
    /// </summary>
    public virtual DbSet<UploadFileType> UploadFileTypes { get; set; }

    /// <summary>
    /// 即时通讯消息内容
    /// </summary>
    public virtual DbSet<Messages.Message> Messages { get; set; }

    /// <summary>
    /// 用户关联对象
    /// </summary>
    public virtual DbSet<Authorization.UserRelations.UserRelation> UserRelations { get; set; }

    /// <summary>
    /// 部门角色
    /// </summary>
    public virtual DbSet<DeptRole> DeptRoles { get; set; }

    /// <summary>
    /// 系统任务
    /// </summary>
    public virtual DbSet<TaskScheduler> TaskSchedulers { get; set; }

    /// <summary>
    /// 系统任务历史记录
    /// </summary>
    public virtual DbSet<TaskExecutionHistory> TaskExecutionHistorys { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    public virtual DbSet<Base_Area> Base_Areas { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    public virtual DbSet<Base_Country> Base_Countrys { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    public virtual DbSet<Base_Province> Base_Provinces { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public virtual DbSet<Base_City> Base_Citys { get; set; }

    /// <summary>
    /// 区县
    /// </summary>
    public virtual DbSet<Base_County> Base_Countys { get; set; }

    #region 客户相关
    /// <summary>
    /// 客户信息
    /// </summary>
    public virtual DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// 客户联系人
    /// </summary>
    public virtual DbSet<CustomerContacts> CustomerContacts { get; set; }

    /// <summary>
    /// 客户跟进记录
    /// </summary>
    public virtual DbSet<CustomerFollowRecord> CustomerFollowRecords { get; set; }

    /// <summary>
    /// 客户产品
    /// </summary>
    public virtual DbSet<CustomerProduct> CustomerProducts { get; set; }

    /// <summary>
    /// 客户分享记录
    /// </summary>
    public virtual DbSet<CustomerShareRecord> CustomerShareRecords { get; set; }

    /// <summary>
    /// 客户操作日志记录
    /// </summary>
    public virtual DbSet<CustomerLogs> CustomerLogs { get; set; }
    #endregion

    /// <summary>
    /// 数据权限
    /// </summary>
    public virtual DbSet<DataAuthorize> DataAuthorizes { get; set; }

    #region 员工信息
    /// <summary>
    /// 员工基础信息表
    /// </summary>
    public virtual DbSet<EmployeeInfo> EmployeeInfos { get; set; }
    /// <summary>
    /// 员工家庭信息表
    /// </summary>
    public virtual DbSet<EmployeeFamilies> EmployeeFamilies { get; set; }
    /// <summary>
    /// 员工合同信息表
    /// </summary>
    public virtual DbSet<EmployeeContracts> EmployeeContracts { get; set; }

    /// <summary>
    /// 员工变更记录
    /// </summary>
    public virtual DbSet<EmployeeChangeLog> EmployeeChangeLogs { get; set; }

    /// <summary>
    /// 员工公司号码
    /// </summary>
    public virtual DbSet<CompanyPhone> CompanyPhones { get; set; }

    #endregion

    #region 考勤相关表
    /// <summary>
    /// 考勤位置表
    /// </summary>
    public virtual DbSet<AttendanceLocation> AttendanceLocations { get; set; }
    /// <summary>
    /// 考勤时间规则表
    /// </summary>
    public virtual DbSet<AttendanceTimeRule> AttendanceTimeRules { get; set; }
    /// <summary>
    /// 考勤时间
    /// </summary>
    public virtual DbSet<AttendanceTime> AttendanceTimes { get; set; }
    /// <summary>
    /// 考勤状态
    /// </summary>
    public virtual DbSet<AttendanceLog> AttendanceLogs { get; set; }
    /// <summary>
    /// 钉钉考勤记录
    /// </summary>
    public virtual DbSet<DingdingUserAttLog> DingdingUserAttLogs { get; set; }
    /// <summary>
    /// 餐补信息
    /// </summary>
    public virtual DbSet<AttendancerMealStatistic> AttendancerMealStatistics { get; set; }
    /// <summary>
    /// 考勤公休日
    /// </summary>
    public virtual DbSet<PublicHoliDay> PublicHoliDays { get; set; }
    /// <summary>
    /// 考勤机
    /// </summary>
    public virtual DbSet<AttendanceMachines> AttendanceMachines { get; set; }

    /// <summary>
    /// 考勤机-记录
    /// </summary>
    public virtual DbSet<CHECKINOUT> CHECKINOUT { get; set; }
    /// <summary>
    /// 考勤机-用户
    /// </summary>
    public virtual DbSet<USERINFO>  USERINFO { get; set; }

    #endregion

    #region 项目相关
    /// <summary>
    /// 项目信息
    /// </summary>
    public virtual DbSet<ProjectInfo> ProjectInfos { get; set; }

    /// <summary>
    /// 项目联系人
    /// </summary>
    public virtual DbSet<ProjectContacts> ProjectContact { get; set; }

    /// <summary>
    /// 项目合同
    /// </summary>
    public virtual DbSet<ProjectContract> ProjectContracts { get; set; }

    /// <summary>
    /// 项目跟进记录
    /// </summary>
    public virtual DbSet<ProjectFollowRecord> ProjectFollowRecords { get; set; }

    /// <summary>
    /// 项目操作日志记录
    /// </summary>
    public virtual DbSet<ProjectLog> ProjectLogs { get; set; }



    #endregion

    #region 流程动态表单
    /// <summary>
    /// 考勤异常
    /// </summary>
    public virtual DbSet<Adto_Att> Adto_Atts { get; set; }
    /// <summary>
    /// 外出申请
    /// </summary>
    public virtual DbSet<Adto_Out> Adto_Outs { get; set; }
    /// <summary>
    /// 出差申请
    /// </summary>
    public virtual DbSet<Adto_Onb> Adto_Onbs { get; set; }
    /// <summary>
    /// 请假申请
    /// </summary>
    public virtual DbSet<Adto_Abs> Adto_Abs { get; set; }
    /// <summary>
    /// 邮箱申请
    /// </summary>
    public virtual DbSet<Adto_EmailRequireForm> Adto_EmailRequireForms { get; set; }
    /// <summary>
    /// 自由呈签
    /// </summary>
    public virtual DbSet<Adto_FreeInformalPetition> Adto_FreeInformalPetition { get; set; }
    /// <summary>
    /// 呈签申请
    /// </summary>
    public virtual DbSet<Adto_InformalPetition> Adto_InformalPetitions { get; set; }

    /// <summary>
    /// OA制度发布
    /// </summary>
    public virtual DbSet<Adto_OaPolicyApproval> Adto_OaPolicyApprovals { get; set; }
    /// <summary>
    /// 物品申请表包括（电脑、手机、总裁办工资及总裁办其他办公用品申请）
    /// </summary>
    public virtual DbSet<Adto_StockApplication> Adto_MerchandiseInventoryApplys { get; set; }

    /// <summary>
    /// 物品申请明细表包括（电脑、手机、总裁办工资及总裁办其他办公用品申请）
    /// </summary>
    public virtual DbSet<Adto_StockApplicationItem> Adto_MerchandiseInventoryApplyDetails { get; set; }

    /// <summary>
    /// 总裁办物品申请
    /// </summary>
    public virtual DbSet<Adto_OfficeSupplyApplication> Adto_OfficeSupplyApplications { get; set; }

    /// <summary>
    /// 总裁办物品申请明细表
    /// </summary>
    public virtual DbSet<Adto_OfficeSupplyApplicationItem> Adto_OfficeSupplyApplicationItems { get; set; }
    /// <summary>
    /// 培训请假申请单
    /// </summary>
    public virtual DbSet<Adto_TrainingRequireForm> Adto_TrainingRequireForms { get; set; }
    /// <summary>
    /// 会议室申请
    /// </summary>
    public virtual DbSet<Adto_TrainingRoomRequireForm> Adto_TrainingRoomRequireForms { get; set; }
    /// <summary>
    /// 用车申请表
    /// </summary>
    public virtual DbSet<Adto_UseCar> Adto_UseCars { get; set; }
    /// <summary>
    /// 加班申请
    /// </summary>
    public virtual DbSet<Adto_WorkOverTime> Adto_WorkOverTimes { get; set; }
    /// <summary>
    /// 流程禁用
    /// </summary>
    public virtual DbSet<DisableUserWrokFlow> DisableUserWrokFlows { get; set; }
    /// <summary>
    /// 请假、外出、考勤异常、出差，申请时间限制表
    /// </summary>
    public virtual DbSet<ApplicationFormCheckDate> ApplicationFormCheckDates { get; set; }

    #endregion

    #region 集成OPENIDDICT
    public virtual DbSet<OpenIddictApplication> Applications { get; set; }

    public virtual DbSet<OpenIddictAuthorization> Authorizations { get; set; }

    public virtual DbSet<OpenIddictScope> Scopes { get; set; }

    public virtual DbSet<OpenIddictToken> Tokens { get; set; }
    #endregion

    #region 新闻资讯
    /// <summary>
    /// 新闻资讯
    /// </summary>
    public virtual DbSet<NewsEntity> NewsEntitys { get; set; }
    /// <summary>
    /// 浏览记录
    /// </summary>
    public virtual DbSet<NewsViewLog> NewsViewLogs { get; set; }

    #endregion

    #region 4P测试
    /// <summary>
    /// 4P测试题库
    /// </summary>
    public virtual DbSet<OA4PQuestion> OA4PQuestions { get; set; }
    /// <summary>
    /// 4p测试记录
    /// </summary>
    public virtual DbSet<OA4PStatistic> OA4PStatistics { get; set; }

    #endregion

    #region 库存管理
    /// <summary>
    /// 库存管理
    /// </summary>
    public virtual DbSet<CommodityStock> CommodityStocks { get; set; }
    /// <summary>
    /// 库存类别
    /// </summary>
    public virtual DbSet<CommodityStockCategory> CommodityStockCategorys { get; set; }
    /// <summary>
    /// 库存操作记录
    /// </summary>
    public virtual DbSet<CommodityStocksRecord> CommodityStocksRecords { get; set; }
    #endregion

    #region 数据库链接

    /// <summary>
    /// 数据链接管理表
    /// </summary>
    public virtual DbSet<DataConnections> DataConnections { get; set; }

    /// <summary>
    /// 数据表管理（导入表源表）
    /// </summary>
    public virtual DbSet<ImportTableSource> ImportTableSources { get; set; }

    #endregion

    #region 单据编码
    /// <summary>
    /// 单据编码配置表
    /// </summary>
    public virtual DbSet<CodeRule.CodeRule> CodeRules { get; set; }

    /// <summary>
    /// 单据编码生成的记录表
    /// </summary>
    public virtual DbSet<CodeRule.CodeRuleRecord> CodeRuleRecords { get; set; }

    #endregion

    #region Excel 导入导出

    /// <summary>
    /// excel 导入配置表
    /// </summary>
    public virtual DbSet<ExcelImport> ExcelImport { get; set; }

    /// <summary>
    /// excel 导入字段设置表
    /// </summary>
    public virtual DbSet<ExcelImportField> ExcelImportField { get; set; }

    /// <summary>
    /// excel 导出配置表
    /// </summary>
    public virtual DbSet<ExcelExport> ExcelExports { get; set; }

    /// <summary>
    /// excel 导出字段设置表
    /// </summary>
    public virtual DbSet<ExcelExportField> ExcelExportFields { get; set; }

    /// <summary>
    /// excel 导出请求参数表
    /// </summary>
    public virtual DbSet<ExcelExportParam> ExcelExportParams { get; set; }

    #endregion

    #region 文件共享
    public DbSet<SharedFileCategory> SharedFileCategorys { get; set; }
    public DbSet<SharedFileInfo> SharedFileInfos { get; set; }
    public DbSet<SharedFileAuthorizes> SharedFileAuthorizes { get; set; }

    #endregion

    /// <summary>
    /// 考卷表
    /// </summary>
    public virtual DbSet<Survey> Surveys { get; set; }

    /// <summary>
    /// 答卷
    /// </summary>
    public virtual DbSet<SurveyAnswer> SurveyAnswers { get; set; }

    /// <summary>
    /// 问卷明细
    /// </summary>
    public virtual DbSet<SurveyAnswerDetail> SurveyAnswerDetails { get; set; }

    /// <summary>
    /// 考卷参与者
    /// </summary>
    public virtual DbSet<SurveyParticipant> SurveyParticipants { get; set; }

    /// <summary>
    /// 题库
    /// </summary>
    public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }

    /// <summary>
    /// 题库类别
    /// </summary>
    public virtual DbSet<SurveyQuestionCategory> SurveyQuestionCategorys { get; set; }


    #region 培训管理

    /// <summary>
    /// 员工培训记录
    /// </summary>
    public DbSet<EmployeesTrainingArchives> EmployeesTrainingArchives { get; set; }

    /// <summary>
    /// 培训文件库
    /// </summary>
    public DbSet<TrainingDoc> TrainingDocs { get; set; }

    /// <summary>
    /// 培训文件类型
    /// </summary>
    public DbSet<TrainingDocCategory> TrainingDocCategorys { get; set; }

    #endregion



    public DCloudDbContext(DbContextOptions<DCloudDbContext> options)
        : base(options)
    {

    }

    /// <summary>
    /// 数据表的主键字段赋值，如果是long类型的主键使用雪花ID,
    /// </summary>
    /// <param name="entry"></param>
    protected override void CheckAndSetId(EntityEntry entry)
    {
        if (entry.Entity is IEntity<long> entity && entity.Id == 0)
        {
            PropertyEntry propertyEntry = entry.Property("Id");
            if (propertyEntry != null && propertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
            {
                var snowflakeFactory = IocManager.Instance.Resolve<ISnowflakeFactory>();
                var snowflake = snowflakeFactory.Get(entry.Metadata.GetTableName());
                entity.Id = snowflake.NextId();
            }
            return;
        }
        base.CheckAndSetId(entry);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        base.OnModelCreating(modelBuilder);

        modelBuilder.ChangeADTOSharpTablePrefix<Tenant, Role, User>("");
        modelBuilder.ConfigureOpenIddict();
        //modelBuilder.Entity<Edition>().ToTable("Editions");

        modelBuilder.Entity<BinaryObject>(b => { b.HasIndex(e => new { e.TenantId }); });

        modelBuilder.Entity<User>().Property(a => a.EmailAddress).IsRequired(false);
        modelBuilder.Entity<User>().Property(a => a.NormalizedEmailAddress).IsRequired(false);

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
            entity.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
            entity.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
            entity.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.UserId });
            entity.HasIndex(e => new { e.TenantId, e.FriendUserId });
            entity.HasIndex(e => new { e.FriendTenantId, e.UserId });
            entity.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasIndex(e => new { e.SubscriptionEndDateUtc });
            entity.HasIndex(e => new { e.CreationTime });
        });

        modelBuilder.Entity<UserDelegation>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.SourceUserId });
            entity.HasIndex(e => new { e.TenantId, e.TargetUserId });
        });

        modelBuilder.Entity<SubscriptionPayment>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<SubscriptionPaymentExtensionData>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<EmployeeInfo>()
        .HasOne(e => e.Department)
        .WithMany()
        .HasForeignKey(e => e.DepartmentId)
        .OnDelete(DeleteBehavior.NoAction);


    }

}

