namespace ADTO.DCloud.Authorization;

/// <summary>
/// 定义应用程序的权限名称的字符串常量。
/// <see cref="DCloudAuthorizationProvider"/> for permission definitions.
/// </summary>
public static class PermissionNames
{
    //公共权限（适用于租户和主机）

    public const string Pages = "Pages";
    public const string Pages_Resources = "Pages.Resources";
    public const string Pages_Administration = "Pages.Administration";

    public const string Pages_DemoUiComponents = "Pages.DemoUiComponents";
    public const string Pages_Administration_Roles = "Pages.Administration.Roles";
    public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
    public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
    public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";

    public const string Pages_Administration_Users = "Pages.Administration.Users";
    public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
    public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
    public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
    public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
    public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";
    public const string Pages_Administration_Users_Unlock = "Pages.Administration.Users.Unlock";
    public const string Pages_Administration_Users_ChangeProfilePicture = "Pages.Administration.Users.ChangeProfilePicture";

    public const string Pages_Administration_Languages = "Pages.Administration.Languages";
    public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
    public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
    public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
    public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";
    public const string Pages_Administration_Languages_ChangeDefaultLanguage = "Pages.Administration.Languages.ChangeDefaultLanguage";

    public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

    public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
    public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
    public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";
    public const string Pages_Administration_OrganizationUnits_ManageRoles = "Pages.Administration.OrganizationUnits.ManageRoles";

    public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";

    public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";

    public const string Pages_Administration_WebhookSubscription = "Pages.Administration.WebhookSubscription";
    public const string Pages_Administration_WebhookSubscription_Create = "Pages.Administration.WebhookSubscription.Create";
    public const string Pages_Administration_WebhookSubscription_Edit = "Pages.Administration.WebhookSubscription.Edit";
    public const string Pages_Administration_WebhookSubscription_ChangeActivity = "Pages.Administration.WebhookSubscription.ChangeActivity";
    public const string Pages_Administration_WebhookSubscription_Detail = "Pages.Administration.WebhookSubscription.Detail";
    public const string Pages_Administration_Webhook_ListSendAttempts = "Pages.Administration.Webhook.ListSendAttempts";
    public const string Pages_Administration_Webhook_ResendWebhook = "Pages.Administration.Webhook.ResendWebhook";

    public const string Pages_Administration_DynamicProperties = "Pages.Administration.DynamicProperties";
    public const string Pages_Administration_DynamicProperties_Create = "Pages.Administration.DynamicProperties.Create";
    public const string Pages_Administration_DynamicProperties_Edit = "Pages.Administration.DynamicProperties.Edit";
    public const string Pages_Administration_DynamicProperties_Delete = "Pages.Administration.DynamicProperties.Delete";

    public const string Pages_Administration_DynamicPropertyValue = "Pages.Administration.DynamicPropertyValue";
    public const string Pages_Administration_DynamicPropertyValue_Create = "Pages.Administration.DynamicPropertyValue.Create";
    public const string Pages_Administration_DynamicPropertyValue_Edit = "Pages.Administration.DynamicPropertyValue.Edit";
    public const string Pages_Administration_DynamicPropertyValue_Delete = "Pages.Administration.DynamicPropertyValue.Delete";

    public const string Pages_Administration_DynamicEntityProperties = "Pages.Administration.DynamicEntityProperties";
    public const string Pages_Administration_DynamicEntityProperties_Create = "Pages.Administration.DynamicEntityProperties.Create";
    public const string Pages_Administration_DynamicEntityProperties_Edit = "Pages.Administration.DynamicEntityProperties.Edit";
    public const string Pages_Administration_DynamicEntityProperties_Delete = "Pages.Administration.DynamicEntityProperties.Delete";

    public const string Pages_Administration_DynamicEntityPropertyValue = "Pages.Administration.DynamicEntityPropertyValue";
    public const string Pages_Administration_DynamicEntityPropertyValue_Create = "Pages.Administration.DynamicEntityPropertyValue.Create";
    public const string Pages_Administration_DynamicEntityPropertyValue_Edit = "Pages.Administration.DynamicEntityPropertyValue.Edit";
    public const string Pages_Administration_DynamicEntityPropertyValue_Delete = "Pages.Administration.DynamicEntityPropertyValue.Delete";
    
    public const string Pages_Administration_MassNotification = "Pages.Administration.MassNotification";
    public const string Pages_Administration_MassNotification_Create = "Pages.Administration.MassNotification.Create";
    
    public const string Pages_Administration_NewVersion_Create = "Pages_Administration_NewVersion_Create";
    //菜单
    public const string Pages_Administration_Modules = "Pages.Administration.Modules";

    public const string Pages_Administration_Dataitems = "Pages.Administration.Dataitems";

    //岗位
    public const string Pages_Administration_Posts = "Pages.Administration.Posts";
    //部门角色
    public const string Pages_Administration_OrganizationUnits_DeptRoles = "Pages.Administration.OrganizationUnits.DeptRoles";
    public const string Pages_Administration_Task_TaskManagement = "Pages.Administration.Task.TaskManagement";//任务管理
    public const string Pages_Administration_System_Settings = "Pages.Administration.System.Settings";//系统设置
    //字段设置
    public const string Pages_Administration_System_FieldSet = "Pages.Administration.System.FieldSet";

    //客户相关的权限
    public const string Pages_Administration_Customers = "Pages.Administration.Customers";
    public const string Pages_Administration_Customers_Create = "Pages.Administration.Customers.Create";
    public const string Pages_Administration_Customers_Edit = "Pages.Administration.Customers.Edit";
    public const string Pages_Administration_Customers_Delete = "Pages.Administration.Customers.Delete";
    public const string Pages_Administration_Customers_Audit = "Pages.Administration.Customers.Audit";
    public const string Pages_Administration_Customers_Shared = "Pages.Administration.Customers.Shared";
    public const string Pages_Administration_Customers_Follow = "Pages.Administration.Customers.Follow";
    public const string Pages_Administration_Customers_Contact = "Pages.Administration.Customers.Contact";

    //流程权限

    public const string Pages_Workflow = "Pages.Workflow";//流程设计
    public const string Pages_Workflow_FlowDesign = "Pages.Workflow.FlowDesign";//流程设计
    public const string Pages_Workflow_FormDesign = "Pages.Workflow.FormDesign";//流程表单设计
    //流程委托
    public const string Pages_Workflow_Delegates = "Pages.Workflow.Delegates";
    public const string Pages_Workflow_Delegates_Create = "Pages.Workflow.Delegates.Create";
    public const string Pages_Workflow_Delegates_Edit = "Pages.Workflow.Delegates.Edit";
    public const string Pages_Workflow_Delegates_Delete = "Pages.Workflow.Delegates.Delete";

    public const string Pages_Workflow_Process = "Pages.Workflow.WorkFlowProcess";//流程申请
    public const string Pages_Workflow_WorkFlowSchemeinfo = "Pages.Workflow.WorkFlowSchemeinfo";//流程模板
    public const string Pages_Workflow_WorkFlowTask = "Pages.Workflow.WorkFlowTask";//流程任务
    public const string Pages_Workflow_WorkFlowTaskLog = "Pages.Workflow.WorkFlowTaskLog";//流程任务日志


    public const string Pages_Workflow_Monitor = "Pages.Workflow.Monitor";
    public const string Pages_Workflow_Monitor_Delete = "Pages.Workflow.Monitor.Delete";//删除流程
    public const string Pages_Workflow_Monitor_PointUser = "Pages.Workflow.Monitor.PointUser";//修改审批人
    public const string Pages_Workflow_Monitor_UpdateProcessScheme = "Pages.Workflow.Monitor.UpdateProcessScheme";//修改流程配置信息
    public const string Pages_Workflow_Monitor_UpdateWorkFlowUser = "Pages.Workflow.Monitor.UpdateWorkFlowUser";//修改流程审批人
    public const string Pages_Workflow_Monitor_PlaceOnFile = "Pages.Workflow.Monitor.PlaceOnFile";//流程归档



    //项目相关权限
    public const string Pages_Administration_Project = "Pages.Administration.Project";
    public const string Pages_Administration_Project_Create = "Pages.Administration.Project.Create";
    public const string Pages_Administration_Project_Edit = "Pages.Administration.Project.Edit";
    public const string Pages_Administration_Project_Delete = "Pages.Administration.Project.Delete";
    public const string Pages_Administration_Project_Audit = "Pages.Administration.Project.Audit";
    public const string Pages_Administration_Project_Contract = "Pages.Administration.Project.Contract";
    public const string Pages_Administration_Project_Follow = "Pages.Administration.Project.Follow";
    public const string Pages_Administration_Project_Contact = "Pages.Administration.Project.Contact";


    //租户相关的权限

    public const string Pages_Tenant_Dashboard = "Pages.Tenant.Dashboard";

    public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";

    public const string Pages_Administration_Tenant_SubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

    //主机相关权限

    public const string Pages_Editions = "Pages.Editions";
    public const string Pages_Editions_Create = "Pages.Editions.Create";
    public const string Pages_Editions_Edit = "Pages.Editions.Edit";
    public const string Pages_Editions_Delete = "Pages.Editions.Delete";
    public const string Pages_Editions_MoveTenantsToAnotherEdition = "Pages.Editions.MoveTenantsToAnotherEdition";

    public const string Pages_Tenants = "Pages.Tenants";
    public const string Pages_Tenants_Create = "Pages.Tenants.Create";
    public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
    public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
    public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
    public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";


    public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
    public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
    public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";


    public const string Pages_Users = "Pages.Users";
    public const string Pages_Roles = "Pages.Roles";


    #region 人资管理权限
    public const string Pages_AdministrationHR = "Pages.AdministrationHR";
    /// <summary>
    /// 员工基本信息
    /// </summary>
    public const string Pages_AdministrationHR_Employee = "Pages.AdministrationHR.Employee";
    public const string Pages_AdministrationHR_Employee_Create = "Pages.AdministrationHR.Employee.Create";
    public const string Pages_AdministrationHR_Employee_Edit = "Pages.AdministrationHR.Employee.Edit";
    public const string Pages_AdministrationHR_Employee_Delete = "Pages.AdministrationHR.Employee.Delete";
    public const string Pages_AdministrationHR_Employee_ResetPassword = "Pages.AdministrationHR.Employee.ResetPassword";//重置密码
    public const string Pages_AdministrationHR_Employee_CompanyPhone = "Pages.AdministrationHR.Employee.CompanyPhone";//公司号码
    public const string Pages_AdministrationHR_Employee_Disable = "Pages.AdministrationHR.Employee.Disable";//禁用/启用
    public const string Pages_AdministrationHR_Employee_AnnualLeaveRecord = "Pages.AdministrationHR.Employee.AnnualLeaveRecord";//年假记录
    public const string Pages_AdministrationHR_Employee_Unlock = "Pages.AdministrationHR.Employee.Unlock";//解锁


    /// <summary>
    /// 员工开户
    /// </summary>
    public const string Pages_AdministrationHR_Employee_OpenAccount = "Pages.AdministrationHR.Employee.OpenAccount"; 
    public const string Pages_AdministrationHR_Employee_OpenAccount_Create = "Pages.AdministrationHR.OpenAccount.Create";
    public const string Pages_AdministrationHR_Employee_OpenAccount_Edit = "Pages.AdministrationHR.OpenAccount.Edit";
    public const string Pages_AdministrationHR_Employee_OpenAccount_Delete = "Pages.AdministrationHR.OpenAccount.Delete";


    #endregion

    #region 新闻权限

    public const string Pages_Administration_Information = "Pages.Administration.Information";
    public const string Pages_Administration_Information_News_Create = "Pages.Administration.Information.News.Create";
    public const string Pages_Administration_Information_News_Delete = "Pages.Administration.Information.News.Delete";
    public const string Pages_Administration_Information_News_Edit = "Pages.Administration.Information.News.Edit";
    public const string Pages_Administration_Information_News_ViewLog = "Pages.Administration.Information.News.ViewLog";


    public const string Pages_Administration_Information_Notice_Create = "Pages.Administration.Information.Notice.Create";
    public const string Pages_Administration_Information_Notice_Delete = "Pages.Administration.Information.Notice.Delete";
    public const string Pages_Administration_Information_Notice_Edit = "Pages.Administration.Information.Notice.Edit";
    public const string Pages_Administration_Information_Notice_ViewLog = "Pages.Administration.Information.Notice.ViewLog";

    public const string Pages_Administration_Information_SteelMarket_Create = "Pages.Administration.Information.SteelMarket.Create";
    public const string Pages_Administration_Information_SteelMarket_Delete = "Pages.Administration.Information.SteelMarket.Delete";
    public const string Pages_Administration_Information_SteelMarket_Edit = "Pages.Administration.Information.SteelMarket.Edit";
    public const string Pages_Administration_Information_SteelMarket_ViewLog = "Pages.Administration.Information.SteelMarket.ViewLog";
    #endregion

    #region 流程表单权限
    public const string Pages_Workflow_ApplicationForm = "Pages.Workflow.ApplicationForm";//流程表单权限
    public const string Pages_Workflow_ApplicationForm_Abs = "Pages.Workflow.ApplicationForm.Abs";//流程表单请假管理
    public const string Pages_Workflow_ApplicationForm_Abs_Edit = "Pages.Workflow.ApplicationForm.Abs.Edit";//流程表单请假管理
    public const string Pages_Workflow_ApplicationForm_Onb = "Pages.Workflow.ApplicationForm.Onb";//流程表单出差管理
    public const string Pages_Workflow_ApplicationForm_Onb_Edit = "Pages.Workflow.ApplicationForm.Abs.Edit";//流程表单出差管理
    public const string Pages_Workflow_ApplicationForm_Out = "Pages.Workflow.ApplicationForm.Out";//流程表单外出管理
    public const string Pages_Workflow_ApplicationForm_Out_Edit = "Pages.Workflow.ApplicationForm.Out.Edit";//流程表单外出管理
    public const string Pages_Workflow_ApplicationForm_Att = "Pages.Workflow.ApplicationForm.Att";//流程表单考勤异常管理
    public const string Pages_Workflow_ApplicationForm_Att_Edit = "Pages.Workflow.ApplicationForm.Abs.Edit";//流程表单考勤异常管理
    public const string Pages_Workflow_ApplicationForm_EmailRequireForm_Edit = "Pages.Workflow.ApplicationForm.EmailRequireForm.Edit";//流程表单邮箱申请管理
    public const string Pages_Workflow_ApplicationForm_FreeInformalPetitions_Edit = "Pages.Workflow.ApplicationForm.FreeInformalPetitions.Edit";//流程表单自由呈签管理
    public const string Pages_Workflow_ApplicationForm_InformalPetitions_Edit = "Pages.Workflow.ApplicationForm.InformalPetitions.Edit";//流程表单合同审批管理
    public const string Pages_Workflow_ApplicationForm_Stocks_Edit = "Pages.Workflow.ApplicationForm.Stocks.Edit";//流程表单电脑申请管理
    public const string Pages_Workflow_ApplicationForm_TrainingRequireForm_Edit = "Pages.Workflow.ApplicationForm.TrainingRequireForm.Edit";//流程表单培训管理
    public const string Pages_Workflow_ApplicationForm_UseCar_Edit = "Pages.Workflow.ApplicationForm.UseCar.Edit";//流程表单用车申请管理
    public const string Pages_Workflow_ApplicationForm_WorkOverTime_Edit = "Pages.Workflow.ApplicationForm.WorkOverTime.Edit";//流程表单加班申请管理
    public const string Pages_Workflow_ApplicationForm_MerchandiseApplication_Edit = "Pages.Workflow.ApplicationForm.MerchandiseApplication.Edit";//流程表单物品申请管理
    public const string Pages_Workflow_ApplicationForm_TrainingRoomRequireForm_Edit = "Pages.Workflow.ApplicationForm.TrainingRoomRequireForm.Edit";//流程表单会议室申请管理

    #endregion

    #region 考勤状态
    public const string Pages_Attendances = "Pages.Attendances";
    public const string Pages_Attendances_List = "Pages.Attendances.List";

    #endregion
}

