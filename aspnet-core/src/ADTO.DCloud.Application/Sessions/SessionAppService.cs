using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Delegation;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Users.Profile;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Editions;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore.Repositories;
using ADTO.DCloud.Features;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Modules;
using ADTO.DCloud.Modules.Dto;
using ADTO.DCloud.MultiTenancy.Payments;
using ADTO.DCloud.Sessions.Dto;
using ADTO.DistributedLocking;
using ADTOSharp;
using ADTOSharp.Application.Features;
using ADTOSharp.Application.Navigation;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Data;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Web.Configuration;
using ADTOSharp.Web.Models.ADTOSharpUserConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NUglify.Helpers;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;


namespace ADTO.DCloud.Sessions;

/// <summary>
/// 会话管理服务
/// </summary>
public class SessionAppService : DCloudAppServiceBase, ISessionAppService
{
    #region Fields

    private readonly IRepository<ADTO.DCloud.Modules.Module, Guid> _moduleRepository;
    private readonly IRepository<ModuleItem, Guid> _moduleItemRepository;
    private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
    private readonly IUserDelegationConfiguration _userDelegationConfiguration;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly EditionManager _editionManager;
    private readonly ILocalizationContext _localizationContext;
    protected IUserNavigationManager UserNavigationManager { get; }
    protected ILanguageManager LanguageManager { get; }
    private readonly IProfileAppService _profileAppService;
    private readonly IIocResolver _iocResolver;
    private readonly IWebHostEnvironment _webHostEnvironment;
    protected ISettingDefinitionManager _settingDefinitionManager { get; }
    private readonly IPostAppService _postAppService;

    private readonly ADTOSharpUserConfigurationBuilder _adtoUserConfigurationBuilder;

    private readonly ICacheManager _cacheManager;

    protected IADTODistributedLock DistributedLock { get; }

    #endregion

    #region Ctor
    public SessionAppService(
        IRepository<ADTO.DCloud.Modules.Module, Guid> moduleRepository,
        IRepository<ModuleItem, Guid> moduleItemRepository,
        ILanguageManager languageManager,
        IUnitOfWorkManager unitOfWorkManager,
        IUserNavigationManager userNavigationManager,
        IIocResolver iocResolver,
        ISettingDefinitionManager settingDefinitionManager,
        ADTOSharpUserConfigurationBuilder adtoUserConfigurationBuilder,
        IPostAppService postAppService,
        IWebHostEnvironment webHostEnvironment,
        IProfileAppService profileAppService,
        IADTODistributedLock distributedLock,
        EditionManager editionManager, ILocalizationContext localizationContext)
    {
        _moduleRepository = moduleRepository;
        _moduleItemRepository = moduleItemRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _editionManager = editionManager;
        _localizationContext = localizationContext;
        LanguageManager = languageManager;
        UserNavigationManager = userNavigationManager;
        _settingDefinitionManager = settingDefinitionManager;
        _iocResolver = iocResolver;
        _adtoUserConfigurationBuilder = adtoUserConfigurationBuilder;
        _postAppService = postAppService;
        _webHostEnvironment = webHostEnvironment;
        _profileAppService = profileAppService;
        DistributedLock = distributedLock;
    }
    #endregion



    #region Utilities


    private async Task<int> FillUserMenuItems(UserIdentifier user, IList<MenuItemDefinition> menuItemDefinitions, IList<UserMenuItem> userMenuItems)
    {
        var addedMenuItemCount = 0;
        // IocManager.Instance.CreateScope
        using (var scope = _iocResolver.CreateScope())
        {
            var permissionDependencyContext = scope.Resolve<PermissionDependencyContext>();
            permissionDependencyContext.User = user;

            var featureDependencyContext = scope.Resolve<FeatureDependencyContext>();
            featureDependencyContext.TenantId = user == null ? null : user.TenantId;

            foreach (var menuItemDefinition in menuItemDefinitions)
            {
                if (menuItemDefinition.RequiresAuthentication && user == null)
                {
                    continue;
                }

                if (menuItemDefinition.PermissionDependency != null &&
                    (user == null || !(await menuItemDefinition.PermissionDependency.IsSatisfiedAsync(permissionDependencyContext))))
                {
                    continue;
                }

                if (menuItemDefinition.FeatureDependency != null &&
                    (ADTOSharpSession.MultiTenancySide == MultiTenancySides.Tenant || (user != null && user.TenantId != null)) &&
                    !(await menuItemDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext)))
                {
                    continue;
                }

                var userMenuItem = new UserMenuItem(menuItemDefinition, _localizationContext);
                if (menuItemDefinition.IsLeaf || (await FillUserMenuItems(user, menuItemDefinition.Items, userMenuItem.Items)) > 0)
                {
                    userMenuItems.Add(userMenuItem);
                    ++addedMenuItemCount;
                }
            }
        }
        return addedMenuItemCount;
    }

    /// <summary>
    /// 生成路由的层级数据
    /// </summary>
    /// <param name="list"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    private List<JObject> GenerateHierarchicalNav(List<ModuleDto> list, Guid? parentId = null)
    {
        var query = list.Where(w => w.ParentId == parentId);
        var result = new List<JObject>();
        foreach (var item in query)
        {
            // var permission = PermissionManager.GetPermissionOrNull(item.Permission);
            if (string.IsNullOrWhiteSpace(item.Permission) || (PermissionManager.GetPermissionOrNull(item.Permission) != null && PermissionChecker.IsGranted(item.Permission)))
            {
                var menu = new JObject();
                result.Add(menu);
                menu["path"] = item.Path;
                menu["component"] = item.Component;
                if (!string.IsNullOrWhiteSpace(item.Redirect))
                    menu["redirect"] = item.Redirect;
                menu["name"] = item.ModuleName;
                var meta = new JObject();
                meta["title"] = item.DisplayName;
                meta["icon"] = item.Icon;
                meta["hidden"] = false;
                meta["alwaysShow"] = true;
                meta["params"] = null;
                meta["keepAlive"] = item.KeepAlive ?? false;
                menu["meta"] = meta;
                if (!string.IsNullOrWhiteSpace(item.Permission))
                    menu["permission"] = item.Permission;
                var children = GenerateHierarchicalNav(list, item.Id);
                if (children.Count() > 0)
                {
                    menu["children"] = JArray.FromObject(children);
                }
            }

        }
        return result;
    }

    #endregion


    public Guid NewGuid()
    {
        return IocManager.Instance.Resolve<IGuidGenerator>().Create();
    }



    /// <summary>
    /// 获取当前系统的所有权限值,如果用户已登录,结果还含有用户得到的授权KEY值
    /// </summary>
    /// <returns></returns>
    public virtual async Task<ADTOSharpUserAuthConfigDto> GetUserAuthConfig()
    {

        var config = new ADTOSharpUserAuthConfigDto();

        var allPermissionNames = PermissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
        var grantedPermissionNames = new List<string>();

        if (ADTOSharpSession.UserId.HasValue)
        {
            foreach (var permissionName in allPermissionNames)
            {
                if (await PermissionChecker.IsGrantedAsync(permissionName))
                {
                    grantedPermissionNames.Add(permissionName);
                }
            }
        }


        config.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
        config.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
        return config;
    }
    /// <summary>
    /// 获取系统/用户的所有本地化名称值对
    /// </summary>
    /// <returns></returns>
    public virtual ADTOSharpUserLocalizationConfigDto GetUserLocalizationConfig()
    {
        var currentCulture = CultureInfo.CurrentUICulture;
        var languages = LanguageManager.GetActiveLanguages();
        var config = new ADTOSharpUserLocalizationConfigDto
        {
            CurrentCulture = new ADTOSharpUserCurrentCultureConfigDto
            {
                Name = currentCulture.Name,
                DisplayName = currentCulture.DisplayName
            },
            Languages = languages.ToList()
        };

        if (languages.Count > 0)
        {
            config.CurrentLanguage = LanguageManager.CurrentLanguage;
        }

        var sources = LocalizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
        config.Sources = sources.Select(s => new ADTOSharpLocalizationSourceDto
        {
            Name = s.Name,
            Type = s.GetType().Name
        }).ToList();

        config.Values = new Dictionary<string, Dictionary<string, string>>();
        foreach (var source in sources)
        {
            var stringValues = source.GetAllStrings(currentCulture).OrderBy(s => s.Name).ToList();
            var stringDictionary = stringValues
                .ToDictionary(_ => _.Name, _ => _.Value);
            config.Values.Add(source.Name, stringDictionary);
        }
        return config;
    }


    [HiddenApi]
    public virtual async Task<ADTOSharpUserAuthConfigDto> GetUserPermissions()
    {
        var auth = new ADTOSharpUserAuthConfigDto();

        var allPermissionNames = PermissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
        var grantedPermissionNames = new List<string>();

        if (ADTOSharpSession.UserId.HasValue)
        {
            foreach (var permissionName in allPermissionNames)
            {
                if (await PermissionChecker.IsGrantedAsync(permissionName))
                {
                    grantedPermissionNames.Add(permissionName);
                }
            }
        }

        auth.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
        auth.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
        return auth;
    }

    /// <summary>
    /// 获取系统所有菜单+路由
    /// </summary>
    /// <returns></returns>
    public virtual async Task<UserNavConfigDto> GetUserNavConfig()
    {
        var list = await _moduleRepository.GetAllReadonly().Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ToListAsync();
        var menus = (list.Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModuleDto>(item);
            return moduleDto;
        }).ToList());

        var routes = GenerateHierarchicalNav(menus, null);

        return new UserNavConfigDto() { Routes = routes, Menus = menus };
    }

    /// <summary>
    /// 获取系统路由
    /// </summary>
    /// <returns></returns>
    public virtual async Task<UserNavConfigDto> GetUserRouteConfig()
    {
        var list = await _moduleRepository.GetAllReadonly().Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ToListAsync();
        var menus = (list.Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModuleDto>(item);
            return moduleDto;
        }).ToList());

        var routes = GenerateHierarchicalNav(menus, null);

        return new UserNavConfigDto() { Routes = routes };
    }
    /// <summary>
    /// 获取App系统所有菜单+路由
    /// </summary>
    /// <returns></returns>
    public virtual async Task<UserNavConfigDto> GetUserAppNavConfig()
    {
        var list = await _moduleRepository.GetAllReadonly().Where(p => p.IsActive && p.IsShowApp == true).OrderBy(p => p.DisplayOrder).ToListAsync();
        var menus = (list.Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModuleDto>(item);
            return moduleDto;
        }).ToList());

        var routes = GenerateHierarchicalNav(menus, null);

        return new UserNavConfigDto() { Routes = routes, Menus = menus };
    }
    /// <summary>
    /// 获取系统路由
    /// </summary>
    /// <returns></returns>
    public virtual async Task<UserNavConfigDto> GetUserAppRouteConfig()
    {
        var list = await _moduleRepository.GetAllReadonly().Where(p => p.IsActive && p.IsShowApp == true).OrderBy(p => p.DisplayOrder).ToListAsync();
        var menus = (list.Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModuleDto>(item);
            return moduleDto;
        }).ToList());

        var routes = GenerateHierarchicalNav(menus, null);

        return new UserNavConfigDto() { Routes = routes };
    }


    #region Methods

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns></returns>
    [ADTOSharpAuthorize, DisableAuditing]
    public async Task<GetCurrentSessionOutput> GetCurrentLoggedUserInformation()
    {





        var output = new GetCurrentSessionOutput
        {
            Application = new ApplicationInfoDto
            {
                Version = AppVersionHelper.Version,
                ReleaseDate = AppVersionHelper.ReleaseDate,
                Features = new Dictionary<string, bool>(),
            }
        };
        if (ADTOSharpSession.TenantId.HasValue)
        {
            output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
        }

        if (ADTOSharpSession.ImpersonatorTenantId.HasValue)
        {
            output.ImpersonatorTenant = await GetTenantLoginInfo(ADTOSharpSession.ImpersonatorTenantId.Value);
        }

        if (ADTOSharpSession.UserId.HasValue)
        {
            var vv = await GetCurrentUserAsync();
            output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            output.User.PostList = await this._postAppService.GetPostByUser(new Authorization.Posts.Dto.GetPostByUserInput() { UserId = output.User.Id });

            //用户图像（没有图像给一个默认图）
            var imageUser = await _profileAppService.GetProfilePictureByUser(output.User.Id);

            if (string.IsNullOrWhiteSpace(imageUser.ProfilePicture))
            {
                byte[] defaultImageBytes = File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, "Common", "Images", "default-profile-picture.png"));
                output.User.ProfilePicture = Convert.ToBase64String(defaultImageBytes); // string
            }
            else
            {
                output.User.ProfilePicture = imageUser.ProfilePicture;
            }

            //var u = await GetCurrentUserAsync();
            //var cc = await UserManager.GetUserRolesAsync(u);
        }

        if (ADTOSharpSession.ImpersonatorUserId.HasValue)
        {
            output.ImpersonatorUser = ObjectMapper.Map<UserLoginInfoDto>(await GetImpersonatorUserAsync());
        }
        return output;
    }

    public ActiveTransactionProviderArgs ActiveTransactionProviderArgs
    {
        get
        {
            return new ActiveTransactionProviderArgs
            {
                ["ContextType"] = typeof(DCloudDbContext),
                ["MultiTenancySide"] = MultiTenancySides.Tenant
            };
        }
    }




    [DisableAuditing, UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
    public virtual async Task<IEnumerable<string>> GetSqlColName(string sqlText, string whereText = null)
    {
        try
        {
            StringBuilder sqlbuf = new StringBuilder(sqlText);
            if (!string.IsNullOrWhiteSpace(whereText))
            {
                if (sqlText.Contains("WHERE"))
                    sqlbuf.Append($" AND {whereText}");
                else
                    sqlbuf.Append($" WHERE {whereText}");
            }
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlbuf.ToString();

                    await connection.OpenAsync();
                    using (var reader = command.ExecuteReader())
                    {

                        //return Enumerable.Range(0, reader.FieldCount)
                        //                            .Select(reader.GetName) 
                        //                            .ToList();
                        return Enumerable.Range(0, reader.FieldCount)
                                    .Select(i => reader.GetName(i).ToLower())
                                    .ToList();
                    }
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }


    public virtual async Task<UserSettingConfigDto> GetUserSettingConfig()
    {
        var config = new UserSettingConfigDto
        {
            Values = new Dictionary<string, string>()
        };

        var settings = await SettingManager.GetAllSettingValuesAsync(SettingScopes.All);

        using (var scope = _iocResolver.CreateScope())
        {
            foreach (var settingValue in settings)
            {
                if (!await _settingDefinitionManager.GetSettingDefinition(settingValue.Name).ClientVisibilityProvider
                    .CheckVisible(scope))
                {
                    continue;
                }

                config.Values.Add(settingValue.Name, settingValue.Value);
            }
        }

        return config;
    }

    /// <summary>
    /// 分布式锁测试
    /// </summary>
    /// <returns></returns>
    public async Task TestDistributedLock()
    {
        await using (var handle = await DistributedLock.TryAcquireAsync(nameof(SessionAppService)))
        {
            if (handle != null)
            {
                //在这里执行的代码,由REDIS来管控全局可入,在同一个时间点内,不管多少个节点,只有一个节点可以进入这里.

            }
        }
    }

    /// <summary>
    /// 获取当前会话信息
    /// </summary>
    /// <returns></returns>
    [DisableAuditing]
    public async Task<GetCurrentSessionOutput> GetCurrentSessionAsync()
    {



        var output = new GetCurrentSessionOutput
        {
            Application = new ApplicationInfoDto
            {
                Version = AppVersionHelper.Version,
                ReleaseDate = AppVersionHelper.ReleaseDate,
                Features = new Dictionary<string, bool>(),
            }
        };

        if (ADTOSharpSession.TenantId.HasValue)
        {
            output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
        }

        if (ADTOSharpSession.ImpersonatorTenantId.HasValue)
        {
            output.ImpersonatorTenant = await GetTenantLoginInfo(ADTOSharpSession.ImpersonatorTenantId.Value);
        }

        if (ADTOSharpSession.UserId.HasValue)
        {
            output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
        }

        if (ADTOSharpSession.ImpersonatorUserId.HasValue)
        {
            output.ImpersonatorUser = ObjectMapper.Map<UserLoginInfoDto>(await GetImpersonatorUserAsync());
        }
        return output;
    }

    /// <summary>
    /// 获取租户信息
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    private async Task<TenantLoginInfoDto> GetTenantLoginInfo(Guid tenantId)
    {
        var tenant = await TenantManager.Tenants
            .Include(t => t.Edition)
            .FirstAsync(t => t.Id == ADTOSharpSession.GetTenantId());

        var tenantLoginInfo = ObjectMapper
            .Map<TenantLoginInfoDto>(tenant);

        if (!tenant.EditionId.HasValue)
        {
            return tenantLoginInfo;
        }

        var features = FeatureManager
            .GetAll()
            .Where(feature => (feature[FeatureMetadata.CustomFeatureKey] as FeatureMetadata)?.IsVisibleOnPricingTable ?? false);

        var featureDictionary = features.ToDictionary(feature => feature.Name, f => f);

        tenantLoginInfo.FeatureValues = (await _editionManager.GetFeatureValuesAsync(tenant.EditionId.Value))
            .Where(featureValue => featureDictionary.ContainsKey(featureValue.Name))
            .Select(fv => new NameValueDto(
                featureDictionary[fv.Name].DisplayName.Localize(_localizationContext),
                featureDictionary[fv.Name].GetValueText(fv.Value, _localizationContext))
            )
            .ToList();

        return tenantLoginInfo;
    }

    private bool IsEditionHighest(Guid editionId, PaymentPeriodType paymentPeriodType)
    {
        var topEdition = GetHighestEditionOrNullByPaymentPeriodType(paymentPeriodType);
        if (topEdition == null)
        {
            return false;
        }

        return editionId == topEdition.Id;
    }

    private SubscribableEdition GetHighestEditionOrNullByPaymentPeriodType(PaymentPeriodType paymentPeriodType)
    {
        var editions = TenantManager.EditionManager.Editions;
        if (editions == null || !editions.Any())
        {
            return null;
        }

        var query = editions.Cast<SubscribableEdition>();

        switch (paymentPeriodType)
        {
            case PaymentPeriodType.Daily:
                query = query.OrderByDescending(e => e.DailyPrice ?? 0);
                break;
            case PaymentPeriodType.Weekly:
                query = query.OrderByDescending(e => e.WeeklyPrice ?? 0);
                break;
            case PaymentPeriodType.Monthly:
                query = query.OrderByDescending(e => e.MonthlyPrice ?? 0);
                break;
            case PaymentPeriodType.Annual:
                query = query.OrderByDescending(e => e.AnnualPrice ?? 0);
                break;
        }

        return query.FirstOrDefault();
    }

    private string GetTenantSubscriptionDateString(GetCurrentSessionOutput output)
    {
        return output.Tenant.SubscriptionEndDateUtc == null
            ? L("Unlimited")
            : output.Tenant.SubscriptionEndDateUtc?.ToString("d");
    }

    /// <summary>
    /// 更新用户的登录凭据
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken()
    {
        if (ADTOSharpSession.UserId == null || ADTOSharpSession.UserId == Guid.Empty)
        {
            throw new Exception(L("ThereIsNoLoggedInUser"));
        }

        var user = await UserManager.GetUserAsync(ADTOSharpSession.ToUserIdentifier());
        user.SetSignInToken();
        return new UpdateUserSignInTokenOutput
        {
            SignInToken = user.SignInToken,
            EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
            EncodedTenantId = user.TenantId.HasValue
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TenantId.Value.ToString()))
                : ""
        };
    }

    /// <summary>
    /// 获取当前会话的模拟帐号
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    protected virtual async Task<User> GetImpersonatorUserAsync()
    {
        using (CurrentUnitOfWork.SetTenantId(ADTOSharpSession.ImpersonatorTenantId))
        {
            var user = await UserManager.FindByIdAsync(ADTOSharpSession.ImpersonatorUserId.ToString());
            if (user == null)
            {
                throw new Exception("User not found!");
            }

            return user;
        }
    }
    #endregion
}

