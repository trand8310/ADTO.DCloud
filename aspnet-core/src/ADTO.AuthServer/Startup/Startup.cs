using ADTO.AspNetCore.OpenIddict;
using ADTO.AuthServer.Resources;
using ADTO.DCloud.Authentication.JwtBearer;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Identity;
using ADTO.DCloud.Web.OpenIddict;
using ADTOSharp.AspNetCore;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Antiforgery;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.AspNetCore.SignalR.Hubs;
using ADTOSharp.Castle.Logging.Log4Net;
using ADTOSharp.Extensions;
using ADTOSharp.PlugIns;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.WebEncoders;
using StackExchange.Redis;
using System.Text.Encodings.Web;
using System.Text.Unicode;



namespace ADTO.AuthServer.Startup;

public class Startup
{
    private readonly IConfigurationRoot _appConfiguration;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private const string _defaultCorsPolicyName = "localhost";

    public Startup(IWebHostEnvironment env)
    {
        _hostingEnvironment = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // MVC
        var mvcBuilder = services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new ADTOSharpAutoValidateAntiforgeryTokenAttribute());
        });

#if DEBUG
        mvcBuilder.AddRazorRuntimeCompilation();
#endif
        ConfigureDataProtection(services);


        var appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Keys");
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(appData))
            .SetApplicationName("dcloud-auth")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));



        IdentityRegistrar.Register(services);
        AuthConfigurer.Configure(services, _appConfiguration);

        if (bool.Parse(_appConfiguration["OpenIddict:IsEnabled"]!))
        {
            OpenIddictRegistrar.Register(services, _appConfiguration, options => { });
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                options => { options.LoginPath = "/Ui/Login"; });
        }
        else
        {
            services.Configure<SecurityStampValidatorOptions>(opts =>
            {
                opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            });
        }







        services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });

        services.AddScoped<IWebResourceManager, WebResourceManager>();

        services.AddSignalR();

        // Configure CORS for angular2 UI
        services.AddCors(
            options => options.AddPolicy(
                _defaultCorsPolicyName,
                builder => builder
                    .WithOrigins(
                        // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                        _appConfiguration["App:CorsOrigins"]!
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )


        );
        // Configure ADTOSharp and Dependency Injection
        services.AddADTOSharpWithoutCreatingServiceProvider<ADTOAuthServerModule>(
            // Configure Log4Net logging
            options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseADTOSharpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                    ? "log4net.config"
                    : "log4net.Production.config"
                )
            )
        );

        //return services.AddADTOSharp<ADTOAuthServerModule>(options =>
        //{
        //    //Configure Log4Net logging
        //    options.IocManager.IocContainer.AddFacility<LoggingFacility>(
        //        f => f.UseADTOSharpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
        //            ? "log4net.config"
        //            : "log4net.Production.config")
        //    );


        //    options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"),
        //        SearchOption.AllDirectories);
        //});
    }

    /// <summary>
    /// 应用数据管理,处理应用重启时之间的TOKEN数据可以继续使用,存在多个服务节点时,有用
    /// </summary>
    /// <param name="services"></param>
    private void ConfigureDataProtection(IServiceCollection services)
    {
        var dataProtectionBuilder = services.AddDataProtection().SetApplicationName("dcloud-auth");
        if (!_hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(_appConfiguration["Configuration:RedisCache:ConnectionString"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "DCloud-Protection-Keys");
        }
        else
        {
            var appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Keys");
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(appData))
                .SetApplicationName("dcloud-api")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
        }
    }




    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseADTOSharp();
        app.UseCors(_defaultCorsPolicyName); // Enable CORS!
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseJwtTokenMiddleware();
        if (bool.Parse(_appConfiguration["OpenIddict:IsEnabled"]!))
        {
            app.UseADTOOpenIddictValidation();
        }
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ADTOSharpCommonHub>("/signalr");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            app.ApplicationServices.GetRequiredService<IADTOSharpAspNetCoreConfiguration>().EndpointConfiguration.ConfigureAllEndpoints(endpoints);
        });
    }
}
