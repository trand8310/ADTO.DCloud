using System;
using System.Linq;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.MultiTenancy;
using ADTOSharp.AspNetCore.Mvc.Auditing;
using ADTOSharp.AspNetCore.Mvc.Caching;
using ADTOSharp.AspNetCore.PlugIn;
using ADTOSharp.AspNetCore.Runtime.Session;
using ADTOSharp.AspNetCore.Security.AntiForgery;
using ADTOSharp.AspNetCore.Webhook;
using ADTOSharp.Auditing;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Web;
using ADTOSharp.Web.Security.AntiForgery;
using ADTOSharp.Webhooks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace ADTOSharp.AspNetCore;

[DependsOn(typeof(ADTOSharpWebCommonModule))]
public class ADTOSharpAspNetCoreModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
        IocManager.AddConventionalRegistrar(new ADTOSharpAspNetCoreConventionalRegistrar());

        IocManager.Register<IADTOSharpAspNetCoreConfiguration, ADTOSharpAspNetCoreConfiguration>();

        Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>(DependencyLifeStyle.Transient);
        Configuration.ReplaceService<IADTOSharpAntiForgeryManager, ADTOSharpAspNetCoreAntiForgeryManager>(DependencyLifeStyle.Transient);
        Configuration.ReplaceService<IClientInfoProvider, HttpContextClientInfoProvider>(DependencyLifeStyle.Transient);
        Configuration.ReplaceService<IWebhookSender, AspNetCoreWebhookSender>(DependencyLifeStyle.Transient);

        IocManager.Register<IGetScriptsResponsePerUserConfiguration, GetScriptsResponsePerUserConfiguration>();

        Configuration.Modules.ADTOSharpAspNetCore().FormBodyBindingIgnoredTypes.Add(typeof(IFormFile));

        Configuration.MultiTenancy.Resolvers.Add<DomainTenantResolveContributor>();
        Configuration.MultiTenancy.Resolvers.Add<HttpHeaderTenantResolveContributor>();
        Configuration.MultiTenancy.Resolvers.Add<HttpCookieTenantResolveContributor>();

        Configuration.Caching.Configure(GetScriptsResponsePerUserCache.CacheName, cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30); });
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpAspNetCoreModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        AddApplicationParts();
        ConfigureAntiforgery();
    }

    private void AddApplicationParts()
    {
        var configuration = IocManager.Resolve<ADTOSharpAspNetCoreConfiguration>();
        var partManager = IocManager.Resolve<ApplicationPartManager>();
        var moduleManager = IocManager.Resolve<IADTOSharpModuleManager>();

        partManager.AddApplicationPartsIfNotAddedBefore(typeof(ADTOSharpAspNetCoreModule).Assembly);

        var controllerAssemblies = configuration.ControllerAssemblySettings.Select(s => s.Assembly).Distinct();
        foreach (var controllerAssembly in controllerAssemblies)
        {
            partManager.AddApplicationPartsIfNotAddedBefore(controllerAssembly);
        }

        var plugInAssemblies = moduleManager.Modules.Where(m => m.IsLoadedAsPlugIn).Select(m => m.Assembly).Distinct();
        foreach (var plugInAssembly in plugInAssemblies)
        {
            partManager.AddADTOSharpPlugInAssemblyPartIfNotAddedBefore(new ADTOSharpPlugInAssemblyPart(plugInAssembly));
        }
    }

    private void ConfigureAntiforgery()
    {
        IocManager.Using<IOptions<AntiforgeryOptions>>(optionsAccessor =>
        {
            optionsAccessor.Value.HeaderName = Configuration.Modules.ADTOSharpWebCommon().AntiForgery.TokenHeaderName;
        });
    }
}