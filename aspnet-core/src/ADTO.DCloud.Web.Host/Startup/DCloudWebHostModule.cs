using ADTO.DCloud.Authentication.External;
using ADTO.DCloud.Authentication.External.Weixin;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Web.Host.Startup.ExternalLoginInfoProviders;
using ADTO.Swashbuckle;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ADTO.DCloud.Web.Host.Startup
{
    [DependsOn(
       typeof(DCloudWebCoreModule),
       typeof(ADTOSwashbuckleModule))]

    public class DCloudWebHostModule : ADTOSharpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public DCloudWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DCloudWebHostModule).GetAssembly());
        }
        public override void PreInitialize()
        {


        }

        public override void PostInitialize()
        {
          
            //using (var scope = IocManager.CreateScope())
            //{
            //    if (!scope.Resolve<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
            //    {
            //        return;
            //    }
            //}

            //var workManager = IocManager.Resolve<IBackgroundWorkerManager>();

            //var tokenCleanupBackgroundWorker = IocManager.Resolve<TokenCleanupBackgroundWorker>();
            //workManager.Add(tokenCleanupBackgroundWorker);
            //注册第三方登录,微信...
            ConfigureExternalAuthProviders();
        }

        private void ConfigureExternalAuthProviders()
        {
            var externalAuthConfiguration = IocManager.Resolve<ExternalAuthConfiguration>();

            if (bool.Parse(_appConfiguration["Authentication:WeixinMiniProgram:IsEnabled"]))
            {
                if (bool.Parse(_appConfiguration["Authentication:AllowSocialLoginSettingsPerTenant"]))
                {
                    externalAuthConfiguration.ExternalLoginInfoProviders.Add(
                        IocManager.Resolve<TenantBasedWeixinMiniProgramExternalLoginInfoProvider>());
                }
                else
                {
                    externalAuthConfiguration.ExternalLoginInfoProviders.Add(new WeixinMiniProgramExternalLoginInfoProvider(
                        _appConfiguration["Authentication:WeixinMiniProgram:AppId"],
                        _appConfiguration["Authentication:WeixinMiniProgram:AppSecret"]
                    ));
                }
            }

        }
    }
}

