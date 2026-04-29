using ADTO.AspNetCore.OpenIddict;
using ADTO.DCloud.Auditing;
using ADTO.DCloud.Authentication.JwtBearer;
using ADTO.DCloud.Authorization.Users.Password;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.FileManage;
using ADTO.DCloud.Media.FileManage.Aliyun;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Web.Configuration;
using ADTO.DistributedLocking;
using ADTOSharp.AspNetCore;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.SignalR;
using ADTOSharp.AspNetCore.WebSocket;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Runtime.Caching.Redis;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace ADTO.DCloud
{
    [DependsOn(
         typeof(DCloudApplicationModule),
         typeof(DCloudEntityFrameworkModule),
         typeof(ADTOSharpAspNetCoreModule),
         typeof(ADTOSharpRedisCacheModule),
         typeof(ADTODistributedLockingModule),
         typeof(ADTOSharpAspNetCoreSignalRModule),
         typeof(ADTOSharpAspNetCoreWebSocketModule),
         typeof(ADTOAspNetCoreOpenIddictModule)

     )]
    public class DCloudWebCoreModule : ADTOSharpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public DCloudWebCoreModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {




            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                DCloudConsts.ConnectionStringName
            );
            #region Redis
            Configuration.Caching.UseRedis(options =>
            {
                options.ConnectionString = _appConfiguration["Configuration:RedisCache:ConnectionString"];
                options.DatabaseId = _appConfiguration.GetValue<int>("Configuration:RedisCache:DatabaseId");
                options.KeyPrefix = "DCloudv2";

                options.OnlineClientsStoreKey = _appConfiguration["Configuration:RedisCache:OnlineClientsStoreKey"]
                                ?? options.OnlineClientsStoreKey;

                options.OnlineClientHeartbeatKeyPrefix = _appConfiguration["Configuration:RedisCache:OnlineClientHeartbeatKeyPrefix"]
                                                         ?? options.OnlineClientHeartbeatKeyPrefix;
                options.OnlineClientInstanceStoreKeyPrefix = _appConfiguration["Configuration:RedisCache:OnlineClientInstanceStoreKeyPrefix"]
                                                             ?? options.OnlineClientInstanceStoreKeyPrefix;
                options.OnlineClientHeartbeatTtlSeconds = _appConfiguration.GetValue<int?>("Configuration:RedisCache:OnlineClientHeartbeatTtlSeconds")
                                                          ?? options.OnlineClientHeartbeatTtlSeconds;
                options.OnlineClientHeartbeatRefreshIntervalSeconds = _appConfiguration.GetValue<int?>("Configuration:RedisCache:OnlineClientHeartbeatRefreshIntervalSeconds")
                                                                      ?? options.OnlineClientHeartbeatRefreshIntervalSeconds;
                options.OnlineClientCleanupIntervalSeconds = _appConfiguration.GetValue<int?>("Configuration:RedisCache:OnlineClientCleanupIntervalSeconds")
                                                             ?? options.OnlineClientCleanupIntervalSeconds;
                options.OnlineClientCleanupBatchSize = _appConfiguration.GetValue<int?>("Configuration:RedisCache:OnlineClientCleanupBatchSize")
                                                       ?? options.OnlineClientCleanupBatchSize;
            });




            #endregion

            //Configuration.Modules.ADTOSharpAspNetCore().UseMvcDateTimeFormatForAppServices = true;

            Configuration.Modules.ADTOSharpAspNetCore().OutputDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //文件管理
            //IocManager.Register<IFileManageService, AliyunFileService>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);

            //IocManager.Register<IFileManageService, LocalFileService>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);

            #region
            //任务

            //IocManager.Register<ITaskSchedulerAppService, TaskSchedulerAppService>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);
            //IocManager.Register<ICycleConfigParser, EveryDayParser>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);
            //IocManager.Register<IDynamicTaskManager, DynamicTaskManager>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);
            //IocManager.Register<ICycleConfigParser, NDayParser>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);

            // 注册任务管理器
            //IocManager.Register<IDynamicTaskManager, DynamicTaskManager>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);





            #endregion

            //Configuration.ReplaceService<IADTODistributedLock, MedallionADTODistributedLock>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);

            Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();
            Configuration.ReplaceService<IAppConfigurationWriter, AppConfigurationWriter>();

            //IocManager.Register(typeof(DapperSqlExecutor<>), typeof(DapperSqlExecutor<>), ADTOSharp.Dependency.DependencyLifeStyle.Transient);
            //IocManager.Register<IDapperSqlExecutor, DapperSqlExecutor<DCloudDbContext>>(ADTOSharp.Dependency.DependencyLifeStyle.Transient);

            Configuration.Modules.ADTOSharpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(DCloudApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();
            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.AccessTokenExpiration = TimeSpan.FromSeconds(Convert.ToInt32(_appConfiguration["Authentication:JwtBearer:AccessTokenExpiration"])); //DCloudConsts.AccessTokenExpiration;
            tokenAuthConfig.RefreshTokenExpiration = TimeSpan.FromSeconds(Convert.ToInt32(_appConfiguration["Authentication:JwtBearer:RefreshTokenExpiration"])); //DCloudConsts.RefreshTokenExpiration;
            tokenAuthConfig.RefreshedAccessTokenExpiration = TimeSpan.FromSeconds(Convert.ToInt32(_appConfiguration["Authentication:JwtBearer:RefreshedAccessTokenExpiration"])); //DCloudConsts.RefreshTokenExpiration;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DCloudWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            SetAppFolders();
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(DCloudWebCoreModule).Assembly);



            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            if (IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                //workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
                //workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
                //workManager.Add(IocManager.Resolve<SubscriptionPaymentNotCompletedEmailNotifierWorker>());
            }

            //删除失效日志
            //var expiredAuditLogDeleterWorker = IocManager.Resolve<ExpiredAuditLogDeleterWorker>();
            //if (Configuration.Auditing.IsEnabled && expiredAuditLogDeleterWorker.IsEnabled)
            //{
            //    workManager.Add(expiredAuditLogDeleterWorker);
            //}
            //密码过期服务
            //workManager.Add(IocManager.Resolve<PasswordExpirationBackgroundWorker>());




            ////定时服务
            //var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
            //workerManager.Add(IocManager.Resolve<ExpiredTestWork>()); 
        }
        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();

            appFolders.SampleProfileImagesFolder = Path.Combine(_env.WebRootPath,
                $"Common{Path.DirectorySeparatorChar}Images{Path.DirectorySeparatorChar}SampleProfilePics");
            appFolders.WebLogsFolder = Path.Combine(_env.ContentRootPath, $"App_Data{Path.DirectorySeparatorChar}Logs");
        }
    }
}

