using ADTOSharp.Authorization;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ADTO.DCloud.Authorization;

/// <summary>
/// 系统权限服务
/// </summary>
public class DCloudAuthorizationProvider : AuthorizationProvider
{
    private readonly bool _isMultiTenancyEnabled;

    public DCloudAuthorizationProvider(bool isMultiTenancyEnabled)
    {
        _isMultiTenancyEnabled = isMultiTenancyEnabled;
    }

    public DCloudAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
    {
        _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
    }

    private string GetFullPath(List<Modules.Module> list, Modules.Module item)
    {
        List<string> paths = new List<string>();
        //if (item.TargetType == "BUTTON")
        //    paths.Add(item.PermissionName);
        //else if (item.Type == "MENU")
        //    paths.Add(item.PermissionName ?? item.RoutePath ?? item.Component);
        //else
        //    paths.Add(item.RoutePath);

        //Modules.Module current = list.FirstOrDefault(p => p.Id == item.ParentId);
        //while (current != null)
        //{
        //    if (current.Type == "BUTTON")
        //        paths.Add(item.PermissionName);
        //    else if (current.Type == "MENU")
        //        paths.Add(current.PermissionName ?? current.RoutePath ?? current.Component);
        //    else
        //        paths.Add(current.RoutePath);
        //    current = list.FirstOrDefault(f => f.Id == current.ParentId);
        //}
        //paths.Reverse();
        return string.Join(".", paths);
    }


    private void GenerateTreeViewList(IPermissionDefinitionContext context, List<Permission> allPermissions, List<Modules.Module> modules, List<Modules.ModuleItem> buttons, Permission parent, Guid? parentId = null)
    {
        if (parentId.HasValue)
        {
            var list = modules.Where(w => w.ParentId == parentId);
            List<string> permissionNames = new List<string>();
            foreach (var item in list)
            {
                if (!item.Permission.IsNullOrWhiteSpace())
                {
                    var modelPermission = parent.CreateChildPermission(item.Permission, L(item.ModuleName));
                    var buttonList = buttons.Where(w => w.ModuleId == item.Id);
                    if (buttonList.Any())
                    {
                        foreach (var button in buttonList)
                        {
                            if (!button.Permission.IsNullOrWhiteSpace())
                            {
                                var permissionName = button.Permission.Contains(".") ? button.Permission : $"{item.Permission}.{button.Permission}";
                                modelPermission.CreateChildPermission(permissionName, L(button.ItemName));
                            }

                        }
                    }
                    if (!item.DynamicPermission.IsNullOrWhiteSpace())
                    {
                        var dynamicPermissionList = item.DynamicPermission.Split(new string[] { System.Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (dynamicPermissionList.Any())
                        {
                            foreach (var dynamic_permission in dynamicPermissionList)
                            {
                                if (!dynamic_permission.IsNullOrWhiteSpace())
                                    modelPermission.CreateChildPermission(dynamic_permission, L(dynamic_permission));
                            }
                        }
                    }

                    GenerateTreeViewList(context, allPermissions, modules, buttons, modelPermission, item.Id);
                }
                else
                {
                    var buttonList = buttons.Where(w => w.ModuleId == item.Id);
                    if (buttonList.Any())
                    {
                        foreach (var button in buttonList)
                        {
                            if (!button.Permission.IsNullOrWhiteSpace())
                            {
                                var permissionName = button.Permission.Contains(".") ? button.Permission : $"{item.Permission}.{button.Permission}";
                                parent.CreateChildPermission(permissionName, L(button.ItemName));
                            }

                        }
                    }
                }


            }
        }
        else
        {
            var list = modules.Where(w => !w.ParentId.HasValue);
            List<string> permissionNames = new List<string>();
            foreach (var item in list)
            {

                if (!item.Permission.IsNullOrWhiteSpace())
                {
                    var modelPermission = parent.CreateChildPermission(item.Permission, L(item.ModuleName));
                    var buttonList = buttons.Where(w => w.ModuleId == item.Id);
                    if (buttonList.Any())
                    {
                        foreach (var button in buttonList)
                        {
                            if (!button.Permission.IsNullOrWhiteSpace())
                            {
                                var permissionName = button.Permission.Contains(".") ? button.Permission : $"{item.Permission}.{button.Permission}";
                                modelPermission.CreateChildPermission(permissionName, L(button.ItemName));
                            }
                        }
                    }

                    if (!item.DynamicPermission.IsNullOrWhiteSpace())
                    {
                        var dynamicPermissionList = item.DynamicPermission.Split(new string[] { System.Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (dynamicPermissionList.Any())
                        {
                            foreach (var dynamic_permission in dynamicPermissionList)
                            {
                                if (!dynamic_permission.IsNullOrWhiteSpace())
                                    modelPermission.CreateChildPermission(dynamic_permission, L(dynamic_permission));
                            }
                        }
                    }


                    GenerateTreeViewList(context, allPermissions, modules, buttons, modelPermission, item.Id);


                }


            }
        }
    }


    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        var pages = context.GetPermissionOrNull(PermissionNames.Pages) ?? context.CreatePermission(PermissionNames.Pages, L("Pages"));
        var resources = pages.CreateChildPermission(PermissionNames.Pages_Resources, L("Resources"));
        SetResourcesPermissions(resources);
        var moduleRepository = IocManager.Instance.Resolve<IRepository<Modules.Module, Guid>>();
        var moduleItemRepository = IocManager.Instance.Resolve<IRepository<Modules.ModuleItem, Guid>>();

        var unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();

        List<Permission> allPermissions = new List<Permission>();
        using (var unitOfWork = unitOfWorkManager.Begin())
        {
            var modules = moduleRepository.GetAllReadonly().ToList();
            var buttons = moduleItemRepository.GetAllReadonly().ToList();
            GenerateTreeViewList(context, allPermissions, modules, buttons, pages);
            unitOfWork.Complete();
        }
        //SetStaticPermissions(context);
    }
    public void SetResourcesPermissions(Permission permission)
    {
     
    }


    public void SetStaticPermissions(IPermissionDefinitionContext context)
    {
        //公共权限（适用于租户和主机）
        #region 公共权限（适用于租户和主机）
        var pages = context.GetPermissionOrNull(PermissionNames.Pages) ?? context.CreatePermission(PermissionNames.Pages, L("Pages"));
        pages.CreateChildPermission(PermissionNames.Pages_DemoUiComponents, L("DemoUiComponents"));

        var administration = pages.CreateChildPermission(PermissionNames.Pages_Administration, L("Administration"));

        var roles = administration.CreateChildPermission(PermissionNames.Pages_Administration_Roles, L("Roles"));
        roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_Create, L("CreatingNewRole"));
        roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_Edit, L("EditingRole"));
        roles.CreateChildPermission(PermissionNames.Pages_Administration_Roles_Delete, L("DeletingRole"));

        var users = administration.CreateChildPermission(PermissionNames.Pages_Administration_Users, L("Users"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Create, L("CreatingNewUser"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Edit, L("EditingUser"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Delete, L("DeletingUser"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Unlock, L("Unlock"));
        users.CreateChildPermission(PermissionNames.Pages_Administration_Users_ChangeProfilePicture, L("UpdateUsersProfilePicture"));

        var languages = administration.CreateChildPermission(PermissionNames.Pages_Administration_Languages, L("Languages"));
        languages.CreateChildPermission(PermissionNames.Pages_Administration_Languages_Create, L("CreatingNewLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
        languages.CreateChildPermission(PermissionNames.Pages_Administration_Languages_Edit, L("EditingLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
        languages.CreateChildPermission(PermissionNames.Pages_Administration_Languages_Delete, L("DeletingLanguages"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
        languages.CreateChildPermission(PermissionNames.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));
        languages.CreateChildPermission(PermissionNames.Pages_Administration_Languages_ChangeDefaultLanguage, L("ChangeDefaultLanguage"));

        administration.CreateChildPermission(PermissionNames.Pages_Administration_AuditLogs, L("AuditLogs"));


        var organizationUnits = administration.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
        organizationUnits.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
        organizationUnits.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
        organizationUnits.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

        administration.CreateChildPermission(PermissionNames.Pages_Administration_UiCustomization, L("VisualSettings"));

        var webhooks = administration.CreateChildPermission(PermissionNames.Pages_Administration_WebhookSubscription, L("Webhooks"));
        webhooks.CreateChildPermission(PermissionNames.Pages_Administration_WebhookSubscription_Create, L("CreatingWebhooks"));
        webhooks.CreateChildPermission(PermissionNames.Pages_Administration_WebhookSubscription_Edit, L("EditingWebhooks"));
        webhooks.CreateChildPermission(PermissionNames.Pages_Administration_WebhookSubscription_ChangeActivity, L("ChangingWebhookActivity"));
        webhooks.CreateChildPermission(PermissionNames.Pages_Administration_WebhookSubscription_Detail, L("DetailingSubscription"));
        webhooks.CreateChildPermission(PermissionNames.Pages_Administration_Webhook_ListSendAttempts, L("ListingSendAttempts"));
        webhooks.CreateChildPermission(PermissionNames.Pages_Administration_Webhook_ResendWebhook, L("ResendingWebhook"));

        var dynamicProperties = administration.CreateChildPermission(PermissionNames.Pages_Administration_DynamicProperties, L("DynamicProperties"));
        dynamicProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicProperties_Create, L("CreatingDynamicProperties"));
        dynamicProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicProperties_Edit, L("EditingDynamicProperties"));
        dynamicProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicProperties_Delete, L("DeletingDynamicProperties"));

        var dynamicPropertyValues = dynamicProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicPropertyValue, L("DynamicPropertyValue"));
        dynamicPropertyValues.CreateChildPermission(PermissionNames.Pages_Administration_DynamicPropertyValue_Create, L("CreatingDynamicPropertyValue"));
        dynamicPropertyValues.CreateChildPermission(PermissionNames.Pages_Administration_DynamicPropertyValue_Edit, L("EditingDynamicPropertyValue"));
        dynamicPropertyValues.CreateChildPermission(PermissionNames.Pages_Administration_DynamicPropertyValue_Delete, L("DeletingDynamicPropertyValue"));

        var dynamicEntityProperties = dynamicProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityProperties, L("DynamicEntityProperties"));
        dynamicEntityProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityProperties_Create, L("CreatingDynamicEntityProperties"));
        dynamicEntityProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityProperties_Edit, L("EditingDynamicEntityProperties"));
        dynamicEntityProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityProperties_Delete, L("DeletingDynamicEntityProperties"));

        var dynamicEntityPropertyValues = dynamicProperties.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityPropertyValue, L("EntityDynamicPropertyValue"));
        dynamicEntityPropertyValues.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Create, L("CreatingDynamicEntityPropertyValue"));
        dynamicEntityPropertyValues.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Edit, L("EditingDynamicEntityPropertyValue"));
        dynamicEntityPropertyValues.CreateChildPermission(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Delete, L("DeletingDynamicEntityPropertyValue"));

        var massNotification = administration.CreateChildPermission(PermissionNames.Pages_Administration_MassNotification, L("MassNotifications"));
        massNotification.CreateChildPermission(PermissionNames.Pages_Administration_MassNotification_Create, L("MassNotificationCreate"));

        var modules = administration.CreateChildPermission(PermissionNames.Pages_Administration_Modules, L("Modules"));
        var dataitems = administration.CreateChildPermission(PermissionNames.Pages_Administration_Dataitems, L("Dataitems"));
        var posts = administration.CreateChildPermission(PermissionNames.Pages_Administration_Posts, L("Posts"));
        var taskManagement = administration.CreateChildPermission(PermissionNames.Pages_Administration_Task_TaskManagement, L("TaskManagement"));
        var systemSettings = administration.CreateChildPermission(PermissionNames.Pages_Administration_System_Settings, L("SystemSettings"));
        var fieldSet = administration.CreateChildPermission(PermissionNames.Pages_Administration_System_FieldSet, L("FieldSet"));
        var deptRoles = administration.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_DeptRoles, L("DeptRoles"));




        var customer = administration.CreateChildPermission(PermissionNames.Pages_Administration_Customers, L("Customers"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Create, L("Customer.Create"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Edit, L("Customer.Edit"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Delete, L("Customer.Delete"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Audit, L("Customer.Audit"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Shared, L("Customer.Shared"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Follow, L("Customer.Follow"));
        customer.CreateChildPermission(PermissionNames.Pages_Administration_Customers_Contact, L("Customer.Contact"));

        var project = administration.CreateChildPermission(PermissionNames.Pages_Administration_Project, L("Project"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Create, L("Project.Create"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Edit, L("Project.Edit"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Delete, L("Project.Delete"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Audit, L("Project.Audit"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Contract, L("Project.Contract"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Follow, L("Project.Follow"));
        project.CreateChildPermission(PermissionNames.Pages_Administration_Project_Contact, L("Project.Contact"));

        //流程
        var workflow = administration.CreateChildPermission(PermissionNames.Pages_Workflow, L("WorkFlow"));

        //流程设计
        var formDesign = administration.CreateChildPermission(PermissionNames.Pages_Workflow_FormDesign, L("WorkFlowFormDesign"));
        //表单设计
        var workflowDesign = administration.CreateChildPermission(PermissionNames.Pages_Workflow_FlowDesign, L("WorkflowDesign"));

        var delegates = administration.CreateChildPermission(PermissionNames.Pages_Workflow_Delegates, L("Delegates"));
        delegates.CreateChildPermission(PermissionNames.Pages_Workflow_Delegates_Create, L("Delegates.Create"));
        delegates.CreateChildPermission(PermissionNames.Pages_Workflow_Delegates_Edit, L("Delegates.Edit"));
        delegates.CreateChildPermission(PermissionNames.Pages_Workflow_Delegates_Delete, L("Delegates.Delete"));


        var workFlowProcess = administration.CreateChildPermission(PermissionNames.Pages_Workflow_Process, L("Process"));
        var workFlowSchemeinfo = administration.CreateChildPermission(PermissionNames.Pages_Workflow_WorkFlowSchemeinfo, L("WorkFlowSchemeinfo"));
        var workFlowTask = administration.CreateChildPermission(PermissionNames.Pages_Workflow_WorkFlowTask, L("WorkFlowTask"));
        #endregion



        //TENANT-SPECIFIC PERMISSIONS
        #region 租户相关的权限
        pages.CreateChildPermission(PermissionNames.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

        administration.CreateChildPermission(PermissionNames.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
        administration.CreateChildPermission(PermissionNames.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

        #endregion

        //HOST-SPECIFIC PERMISSIONS
        #region 主机相关权限
        var editions = pages.CreateChildPermission(PermissionNames.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
        editions.CreateChildPermission(PermissionNames.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
        editions.CreateChildPermission(PermissionNames.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
        editions.CreateChildPermission(PermissionNames.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
        editions.CreateChildPermission(PermissionNames.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

        var tenants = pages.CreateChildPermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        tenants.CreateChildPermission(PermissionNames.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
        tenants.CreateChildPermission(PermissionNames.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
        tenants.CreateChildPermission(PermissionNames.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
        tenants.CreateChildPermission(PermissionNames.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
        tenants.CreateChildPermission(PermissionNames.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

        administration.CreateChildPermission(PermissionNames.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);

        var maintenance = administration.CreateChildPermission(PermissionNames.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
        maintenance.CreateChildPermission(PermissionNames.Pages_Administration_NewVersion_Create, L("SendNewVersionNotification"));

        administration.CreateChildPermission(PermissionNames.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
        administration.CreateChildPermission(PermissionNames.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        #endregion


        #region 人资管理员权限

        var administrationHR = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR, L("AdministrationHR"));
        var employee = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee, L("AdministrationHR.Employee"));
        var employee_Create = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_Create, L("AdministrationHR.Employee.Create"));
        var employee_Edit = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_Edit, L("AdministrationHR.Employee.Edit"));
        var employee_Delete = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_Delete, L("AdministrationHR.Employee.Delete"));
        var employee_ResetPassword = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_ResetPassword, L("AdministrationHR.Employee.ResetPassword"));//重置密码
        var employee_CompanyPhone = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_CompanyPhone, L("AdministrationHR.Employee.CompanyPhone"));//公司号码
        var employee_Disable = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_Disable, L("AdministrationHR.Employee.Disable"));//禁用/启用
        var employee_AnnualLeaveRecord = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_AnnualLeaveRecord, L("AdministrationHR.Employee.AnnualLeaveRecord"));//年假记录
        var employee_Unlock = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_Unlock, L("AdministrationHR.Employee.Unlock"));//解锁


        var openAccount = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_OpenAccount, L("AdministrationHR.OpenAccount"));
        var openAccount_Create = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_OpenAccount_Create, L("AdministrationHR.OpenAccount.Create"));
        var openAccount_Edit = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_OpenAccount_Edit, L("AdministrationHR.OpenAccount.Edit"));
        var openAccount_Delete = administration.CreateChildPermission(PermissionNames.Pages_AdministrationHR_Employee_OpenAccount_Delete, L("AdministrationHR.OpenAccount.Delete"));

        #endregion




    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, DCloudConsts.LocalizationSourceName);
    }


    //public override void SetPermissions(IPermissionDefinitionContext context)
    //{
    //    context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
    //    context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
    //    context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
    //    context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
    //}
}

