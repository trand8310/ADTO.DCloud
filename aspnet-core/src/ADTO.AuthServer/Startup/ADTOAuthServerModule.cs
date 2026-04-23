using ADTO.DCloud;
using ADTO.DCloud.Authentication.External;
using ADTO.DCloud.Authentication.External.Weixin;
using ADTO.DCloud.Configuration;
using ADTO.OpenIddict.Tokens;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Threading.BackgroundWorkers;

namespace ADTO.AuthServer.Startup;


[DependsOn(
    typeof(DCloudWebCoreModule)
    )]
public class ADTOAuthServerModule : ADTOSharpModule
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfigurationRoot _appConfiguration;

    public ADTOAuthServerModule(IWebHostEnvironment env)
    {
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public override void PreInitialize()
    {
        //Configuration.Navigation.Providers.Add<DCloudNavigationProvider>();



        //Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();
        //Configuration.ReplaceService<IAppConfigurationWriter, AppConfigurationWriter>();



    }
    public override void PostInitialize()
    {
        var workManager = IocManager.Resolve<IBackgroundWorkerManager>();

        var tokenCleanupBackgroundWorker = IocManager.Resolve<TokenCleanupBackgroundWorker>();
        workManager.Add(tokenCleanupBackgroundWorker);


        //注册第三方登录,微信...
        ConfigureExternalAuthProviders();
    }
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOAuthServerModule).GetAssembly());
    }

    private void ConfigureExternalAuthProviders()
    {
        var externalAuthConfiguration = IocManager.Resolve<ExternalAuthConfiguration>();

        if (bool.Parse(_appConfiguration["Authentication:WeixinMiniProgram:IsEnabled"]))
        {
            if (bool.Parse(_appConfiguration["Authentication:AllowSocialLoginSettingsPerTenant"]))
            {
                //externalAuthConfiguration.ExternalLoginInfoProviders.Add(
                //    IocManager.Resolve<TenantBasedWeixinMiniProgramExternalLoginInfoProvider>());
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

