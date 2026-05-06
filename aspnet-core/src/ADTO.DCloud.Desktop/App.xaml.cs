using System.IO;
using System.Windows;
using ADTO.DCloud.Desktop.Modules;
using ADTO.DCloud.Desktop.Services;
using ADTO.DCloud.Desktop.ViewModels;
using ADTO.DCloud.Desktop.Views;
using DryIoc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;

namespace ADTO.DCloud.Desktop;

public partial class App
{
    private IHost? _host;

    protected override Window CreateShell()
    {
        return Container.Resolve<LoginWindow>();
    }


    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<OfficeShellModule>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var configuration = BuildConfiguration();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.File("logs/dcloud-desktop-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IConfiguration>(configuration);
                services.AddSingleton<ISessionService, SessionService>();
                services.AddSingleton<IClockService, ClockService>();
                services.AddHostedService<DesktopHeartbeatService>();
                services.AddHttpClient<IAuthApiClient, AuthApiClient>();
            })
            .Build();

        _host.Start();

        containerRegistry.RegisterInstance(configuration);
        containerRegistry.RegisterSingleton<IApplicationNavigator, ApplicationNavigator>();
        containerRegistry.RegisterInstance(_host.Services.GetRequiredService<ISessionService>());
        containerRegistry.RegisterInstance(_host.Services.GetRequiredService<IClockService>());
        containerRegistry.RegisterInstance(_host.Services.GetRequiredService<IAuthApiClient>());

        containerRegistry.Register<LoginWindow>();
        containerRegistry.Register<MainWindow>();
        containerRegistry.Register<LoginViewModel>();
        containerRegistry.Register<MainViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.StopAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
        _host?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "DCLOUD_DESKTOP_")
            .Build();
    }
}
