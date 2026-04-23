using System;
using System.Linq;
using ADTOSharp.AspNetCore.EmbeddedResources;
using ADTOSharp.AspNetCore.Localization;
using ADTOSharp.Dependency;
using ADTOSharp.Localization;
using Castle.LoggingFacility.MsLogging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using ADTOSharp.AspNetCore.ExceptionHandling;
using ADTOSharp.AspNetCore.Security;
using ADTOSharp.AspNetCore.Uow;
using Microsoft.Extensions.Hosting;

namespace ADTOSharp.AspNetCore;

public static class ADTOSharpApplicationBuilderExtensions
{
    private const string AuthorizationExceptionHandlingMiddlewareMarker = "_ADTOSharpAuthorizationExceptionHandlingMiddleware_Added";

    public static void UseADTOSharp(this IApplicationBuilder app)
    {
        app.UseADTOSharp(null);
    }

    public static void UseADTOSharp([NotNull] this IApplicationBuilder app, Action<ADTOSharpApplicationBuilderOptions> optionsAction)
    {
        Check.NotNull(app, nameof(app));

        var options = new ADTOSharpApplicationBuilderOptions();
        optionsAction?.Invoke(options);

        if (options.UseCastleLoggerFactory)
        {
            app.UseCastleLoggerFactory();
        }

        InitializeADTOSharp(app);

        if (options.UseADTOSharpRequestLocalization)
        {
            //TODO: This should be added later than authorization middleware!
            app.UseADTOSharpRequestLocalization();
        }

        if (options.UseSecurityHeaders)
        {
            app.UseADTOSharpSecurityHeaders();
        }
    }

    public static void UseEmbeddedFiles(this IApplicationBuilder app)
    {
        app.UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new EmbeddedResourceFileProvider(
                    app.ApplicationServices.GetRequiredService<IIocResolver>()
                )
            }
        );
    }

    private static void InitializeADTOSharp(IApplicationBuilder app)
    {
        var bootstrapper = app.ApplicationServices.GetRequiredService<ADTOSharpBootstrapper>();
        bootstrapper.Initialize();

        var applicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStopping.Register(() => bootstrapper.Dispose());
    }

    public static void UseCastleLoggerFactory(this IApplicationBuilder app)
    {
        var castleLoggerFactory = app.ApplicationServices.GetService<Castle.Core.Logging.ILoggerFactory>();
        if (castleLoggerFactory == null)
        {
            return;
        }

        app.ApplicationServices
            .GetRequiredService<ILoggerFactory>()
            .AddCastleLogger(castleLoggerFactory);
    }

    public static void UseADTOSharpRequestLocalization(this IApplicationBuilder app, Action<RequestLocalizationOptions> optionsAction = null)
    {
        var iocResolver = app.ApplicationServices.GetRequiredService<IIocResolver>();
        using (var languageManager = iocResolver.ResolveAsDisposable<ILanguageManager>())
        {
            var supportedCultures = languageManager.Object
                .GetActiveLanguages()
                .Select(l => CultureInfo.GetCultureInfo(l.Name))
                .ToArray();

            if (iocResolver.IsRegistered<ILogger<RequestLocalizationOptions>>())
            {
                using (var logger = iocResolver.ResolveAsDisposable<ILogger<RequestLocalizationOptions>>())
                {
                    logger.Object.LogInformation($"Supported Request Localization Cultures: {string.Join(",", supportedCultures.Select(c => c))}");
                }
            }

            var options = new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            var userProvider = new ADTOSharpUserRequestCultureProvider();

            //0: QueryStringRequestCultureProvider
            options.RequestCultureProviders.Insert(1, userProvider);
            options.RequestCultureProviders.Insert(2, new ADTOSharpLocalizationHeaderRequestCultureProvider());
            //3: CookieRequestCultureProvider
            //4: AcceptLanguageHeaderRequestCultureProvider
            options.RequestCultureProviders.Insert(5, new ADTOSharpDefaultRequestCultureProvider());

            optionsAction?.Invoke(options);

            userProvider.CookieProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
            userProvider.HeaderProvider = options.RequestCultureProviders.OfType<ADTOSharpLocalizationHeaderRequestCultureProvider>().FirstOrDefault();

            app.UseRequestLocalization(options);
        }
    }

    public static void UseADTOSharpSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseMiddleware<ADTOSharpSecurityHeadersMiddleware>();
    }

    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<ADTOSharpUnitOfWorkMiddleware>();
    }

    public static IApplicationBuilder UseADTOSharpAuthorizationExceptionHandling(this IApplicationBuilder app)
    {
        if (app.Properties.ContainsKey(AuthorizationExceptionHandlingMiddlewareMarker))
        {
            return app;
        }

        app.Properties[AuthorizationExceptionHandlingMiddlewareMarker] = true;
        return app.UseMiddleware<ADTOSharpAuthorizationExceptionHandlingMiddleware>();
    }
}