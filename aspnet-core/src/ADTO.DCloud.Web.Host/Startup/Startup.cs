using ADTO.DCloud.Authentication.JwtBearer;
using ADTO.DCloud.Chat.WS;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Identity;
using ADTO.DCloud.Web.Chat.SignalR;
using ADTO.DCloud.Web.Host.BackgroundJobs;
using ADTO.DCloud.Web.Swagger;
using ADTO.Swashbuckle;
using ADTOSharp.AspNetCore;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Antiforgery;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.AspNetCore.SignalR.Hubs;
using ADTOSharp.Castle.Logging.Log4Net;
using Castle.Facilities.Logging;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jurassic;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;

namespace ADTO.DCloud.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private const string _apiVersion = "v1";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            //// 注册解析器
            //services.AddSingleton<ICycleConfigParser, EveryDayParser>();
            //services.AddSingleton<ICycleConfigParser, NDayParser>();

            //// 注册 TaskSchedulerAppService（单例）
            //services.AddSingleton<ITaskSchedulerAppService, TaskSchedulerAppService>();

            //// 注册任务管理器（单例）
            //services.AddSingleton<IDynamicTaskManager, DynamicTaskManager>();
            //// 注册周期解析器
            //services.AddDynamicTaskScheduler();
            //// 注册后台服务
            //services.AddHostedService<DynamicTaskBackgroundService>();


            //MVC
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new ADTOSharpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            ConfigureDataProtection(services);
            //var appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data","Keys");
            //services.AddDataProtection()
            //    .PersistKeysToFileSystem(new DirectoryInfo(appData))
            //    .SetApplicationName("dcloud-api")
            //    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));


            #region Redis Backplane

            services.AddSignalR()
                .AddStackExchangeRedis(
                    _appConfiguration["Configuration:RedisCache:ConnectionString"],
                    options =>
                    {
                        options.Configuration.ChannelPrefix = $"adto-signalr:{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";
                        options.Configuration.AbortOnConnectFail = false;
                        options.Configuration.ConnectRetry = 3;
                        options.Configuration.ConnectTimeout = 5000;
                        options.Configuration.SyncTimeout = 5000;
                    });



            #endregion


            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            //services.TryAddEnumerable(new[]
            //{
            //    ServiceDescriptor.Singleton<ICycleConfigParser, EveryDayParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, NDayParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, EveryHourParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, NHourParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, NMinuteParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, EveryWeekParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, EveryMonthParser>(),
            //    ServiceDescriptor.Singleton<ICycleConfigParser, NSecondParser>()
            // });

            // 注册任务管理器
            //services.AddSingleton<IDynamicTaskManager, DynamicTaskManager>();
            // 注册后台服务
            //services.AddHostedService<DynamicTaskBackgroundService>();

            services.AddSignalR();

            services.AddHostedService<SignalROnlineClientJanitorHostedService>();


            // Configure CORS for angular2 UI
            /*
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );
            */

            services.AddCors(options =>
            {
                options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            });


            //配置分布式锁
            ConfigureDistributedLocking(services);


            //启用 Swagger UI
            ConfigureSwagger(services);



            //Configure ADTOSharp and Dependency Injection
            services.AddADTOSharpWithoutCreatingServiceProvider<DCloudWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseADTOSharpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                        ? "log4net.config"
                        : "log4net.Production.config"
                    )
                )
            );


            //services.AddADTOSharp<DCloudWebHostModule>(options =>
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
            var dataProtectionBuilder = services.AddDataProtection().SetApplicationName("dcloud-api");
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
            app.UseADTOSharp(options => { options.UseADTOSharpRequestLocalization = false; });

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseWebSockets();


            app.UseRouting();

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();
            app.UseAuthorization();
            app.UseMiddleware<ChatWebSocketMiddleware>();
            app.UseADTOSharpRequestLocalization();

            // js 脚本引擎
            JsEngineSwitcher.Current.EngineFactories.AddJurassic();
            JsEngineSwitcher.Current.DefaultEngineName = JurassicJsEngine.EngineName;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ADTOSharpCommonHub>("/signalr");
                endpoints.MapHub<ChatHub>("/signalr-chat");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
                app.ApplicationServices.GetRequiredService<IADTOSharpAspNetCoreConfiguration>().EndpointConfiguration.ConfigureAllEndpoints(endpoints);
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            app.UseADTOSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"DCloud API {_apiVersion}");
                //options.InjectJavascript("ui/lang-zh.js"); // 注入上面的翻译脚本
                //var configuration = context.GetConfiguration();
                options.OAuthClientId(_appConfiguration["AuthServer:SwaggerClientId"]);
                options.OAuthScopes("DCloud");
                options.DefaultModelsExpandDepth(-1);

            });



            //// Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            //app.UseSwaggerUI(options =>
            //{
            //    // specifying the Swagger JSON endpoint.
            //    options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"DCloud API {_apiVersion}");
            //    options.IndexStream = () => Assembly.GetExecutingAssembly()
            //        .GetManifestResourceStream("ADTO.DCloud.Web.Host.wwwroot.swagger.ui.index.html");
            //    options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.
            //    options.DefaultModelsExpandDepth(-1);
            //}); // URL: /swagger
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddADTOSwaggerGenWithOidc(
                _appConfiguration["AuthServer:Authority"]!,
                scopes: new[] { "DCloud" },
                // "authorization_code"
                flows: new[] { ADTOSwaggerOidcFlows.AuthorizationCode, ADTOSwaggerOidcFlows.Password },
                // Should be the discovery endpoint of the reachable DNS of the AuthServer over the internet like https://myauthserver.company.com/.well-known/openid-configuration
                // Default null value is the "configuration["AuthServer:Authority"]+.well-known/openid-configuration
                discoveryEndpoint: null,
                options =>
                {
                    options.SwaggerDoc(_apiVersion, new OpenApiInfo { Title = "DCloud API", Version = _apiVersion, Description = "DCloud", });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    options.ParameterFilter<SwaggerEnumParameterFilter>();
                    options.SchemaFilter<SwaggerEnumSchemaFilter>();
                    options.OperationFilter<SwaggerOperationIdFilter>();
                    options.OperationFilter<SwaggerOperationFilter>();
                    options.CustomDefaultSchemaIdSelector();
                    options.DocumentFilter<HiddenApiFilter>();
                    //add summaries to swagger
                    bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
                    if (canShowSummaries)
                    {
                        var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
                        options.IncludeXmlComments(hostXmlPath, true);

                        var applicationXml = $"ADTO.DCloud.Application.xml";
                        var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                        options.IncludeXmlComments(applicationXmlPath, true);

                        var webCoreXmlFile = $"ADTO.DCloud.Web.Core.xml";
                        var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                        options.IncludeXmlComments(webCoreXmlPath, true);

                        options.DocumentFilter<OrderTagsDocumentFilter>();
                        options.OrderActionsBy(o => o.RelativePath);
                    }

                });


            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc(_apiVersion, new OpenApiInfo
            //    {
            //        Version = _apiVersion,
            //        Title = "DCloud API",
            //        Description = "DCloud",
            //    });
            //    options.DocInclusionPredicate((docName, description) => true);
            //    options.ParameterFilter<SwaggerEnumParameterFilter>();
            //    options.SchemaFilter<SwaggerEnumSchemaFilter>();
            //    options.OperationFilter<SwaggerOperationIdFilter>();
            //    options.OperationFilter<SwaggerOperationFilter>();
            //    options.CustomDefaultSchemaIdSelector();
            //    options.DocumentFilter<HiddenApiFilter>();

            //    // Define the BearerAuth scheme that's in use
            //    options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
            //    {
            //        Description =
            //            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            //        Name = "Authorization",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.ApiKey
            //    });

            //    //add summaries to swagger
            //    bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
            //    if (canShowSummaries)
            //    {
            //        var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //        var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
            //        options.IncludeXmlComments(hostXmlPath, true);

            //        var applicationXml = $"ADTO.DCloud.Application.xml";
            //        var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
            //        options.IncludeXmlComments(applicationXmlPath, true);

            //        var webCoreXmlFile = $"ADTO.DCloud.Web.Core.xml";
            //        var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
            //        options.IncludeXmlComments(webCoreXmlPath, true);

            //        options.DocumentFilter<OrderTagsDocumentFilter>();
            //        options.OrderActionsBy(o => o.RelativePath);
            //    }
            //});
        }



        private void ConfigureDistributedLocking(IServiceCollection services)
        {
            services.AddSingleton<IDistributedLockProvider>(option =>
            {
                var connection = ConnectionMultiplexer.Connect(_appConfiguration["Configuration:RedisCache:ConnectionString"]);
                //                options.ConnectionString = _appConfiguration["Configuration:RedisCache:ConnectionString"];
                // options.DatabaseId = _appConfiguration.GetValue<int>("Configuration:RedisCache:DatabaseId");
                return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
            });
        }
    }
}

