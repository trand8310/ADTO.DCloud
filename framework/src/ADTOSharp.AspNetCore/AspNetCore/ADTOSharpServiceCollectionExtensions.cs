using System;
using System.Collections.Generic;
using System.Text.Json;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.EmbeddedResources;
using ADTOSharp.AspNetCore.Mvc;
using ADTOSharp.Dependency;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ADTOSharp.AspNetCore.Mvc.Providers;
using ADTOSharp.AspNetCore.Webhook;
using ADTOSharp.Auditing;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Json.SystemTextJson;
using ADTOSharp.Modules;
using ADTOSharp.Runtime.Validation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace ADTOSharp.AspNetCore;

public static class ADTOSharpServiceCollectionExtensions
{
    /// <summary>
    /// Integrates ADTO to AspNet Core.
    /// </summary>
    /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="ADTOSharpModule"/>.</typeparam>
    /// <param name="services">Services.</param>
    /// <param name="optionsAction">An action to get/modify options</param>
    /// <param name="removeConventionalInterceptors">Removes the conventional interceptors</param>
    public static IServiceProvider AddADTOSharp<TStartupModule>(this IServiceCollection services,
        [CanBeNull] Action<ADTOSharpBootstrapperOptions> optionsAction = null,
        bool removeConventionalInterceptors = true)
        where TStartupModule : ADTOSharpModule
    {
        if (removeConventionalInterceptors)
        {
            RemoveConventionalInterceptionSelectors();
        }

        var adtoBootstrapper = AddADTOSharpBootstrapper<TStartupModule>(services, optionsAction);
        ConfigureAspNetCore(services, adtoBootstrapper.IocManager);

        return WindsorRegistrationHelper.CreateServiceProvider(adtoBootstrapper.IocManager.IocContainer, services);
    }

    /// <summary>
    /// Integrates ADTO to AspNet Core without creating a IServiceProvider.
    /// </summary>
    /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="ADTOSharpModule"/>.</typeparam>
    /// <param name="services">Services.</param>
    /// <param name="optionsAction">An action to get/modify options</param>
    /// <param name="removeConventionalInterceptors">Removes the conventional interceptors</param>
    public static void AddADTOSharpWithoutCreatingServiceProvider<TStartupModule>(this IServiceCollection services,
        [CanBeNull] Action<ADTOSharpBootstrapperOptions> optionsAction = null,
        bool removeConventionalInterceptors = true)
        where TStartupModule : ADTOSharpModule
    {
        if (removeConventionalInterceptors)
        {
            RemoveConventionalInterceptionSelectors();
        }

        var adtoBootstrapper = AddADTOSharpBootstrapper<TStartupModule>(services, optionsAction);
        ConfigureAspNetCore(services, adtoBootstrapper.IocManager);
    }

    private static void RemoveConventionalInterceptionSelectors()
    {
        UnitOfWorkDefaultOptions.ConventionalUowSelectorList = new List<Func<Type, bool>>();
        ADTOSharpAuditingDefaultOptions.ConventionalAuditingSelectorList = new List<Func<Type, bool>>();
        ADTOSharpValidationDefaultOptions.ConventionalValidationSelectorList = new List<Func<Type, bool>>();
    }

    private static void ConfigureAspNetCore(IServiceCollection services, IIocResolver iocResolver)
    {
        //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        //Use DI to create controllers
        services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

        //Use DI to create page models
        services.Replace(ServiceDescriptor
            .Singleton<IPageModelActivatorProvider, ServiceBasedPageModelActivatorProvider>());

        //Use DI to create view components
        services.Replace(ServiceDescriptor
            .Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

        //Add feature providers
        var partManager = services.GetSingletonServiceOrNull<ApplicationPartManager>();
        partManager?.FeatureProviders.Add(new ADTOSharpAppServiceControllerFeatureProvider(iocResolver));

        //Configure System Text JSON serializer
        services.AddOptions<JsonOptions>()
            .Configure<IServiceProvider>((options, rootServiceProvider) =>
        {
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.JsonSerializerOptions.AllowTrailingCommas = true;

            options.JsonSerializerOptions.Converters.Add(new ADTOSharpStringToEnumFactory());
            options.JsonSerializerOptions.Converters.Add(new ADTOSharpStringToBooleanConverter());
            options.JsonSerializerOptions.Converters.Add(new ADTOSharpStringToGuidConverter());
            options.JsonSerializerOptions.Converters.Add(new ADTOSharpNullableStringToGuidConverter());
            options.JsonSerializerOptions.Converters.Add(new ADTOSharpNullableFromEmptyStringConverterFactory());
            options.JsonSerializerOptions.Converters.Add(new ObjectToInferredTypesConverter());

            var aspNetCoreConfiguration = rootServiceProvider.GetRequiredService<IADTOSharpAspNetCoreConfiguration>();
            options.JsonSerializerOptions.TypeInfoResolver = new ADTOSharpDateTimeJsonTypeInfoResolver(aspNetCoreConfiguration.InputDateTimeFormats, aspNetCoreConfiguration.OutputDateTimeFormat);
        });

        //Configure MVC
        services.Configure<MvcOptions>(mvcOptions => { mvcOptions.AddADTOSharp(services); });

        //Configure Razor
        services.Insert(0,
            ServiceDescriptor.Singleton<IConfigureOptions<MvcRazorRuntimeCompilationOptions>>(
                new ConfigureOptions<MvcRazorRuntimeCompilationOptions>(
                    (options) => { options.FileProviders.Add(new EmbeddedResourceViewFileProvider(iocResolver)); }
                )
            )
        );

        services.AddHttpClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName);
    }

    private static ADTOSharpBootstrapper AddADTOSharpBootstrapper<TStartupModule>(IServiceCollection services,
        Action<ADTOSharpBootstrapperOptions> optionsAction)
        where TStartupModule : ADTOSharpModule
    {
        var adtoBootstrapper = ADTOSharpBootstrapper.Create<TStartupModule>(optionsAction);

        services.AddSingleton(adtoBootstrapper);

        return adtoBootstrapper;
    }
}