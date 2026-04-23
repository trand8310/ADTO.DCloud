using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Controllers;
using ADTO.DCloud.Notifications;
using ADTOSharp;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.Notifications;
using ADTOSharp.Timing;
using ADTOSharp.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Web.Host.Controllers;

public class HomeController : DCloudControllerBase
{
    private readonly INotificationPublisher _notificationPublisher;
    private readonly UserManager _userManager;

    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationRoot _appConfiguration;
    private readonly IWebHostEnvironment _env;


    public HomeController(INotificationPublisher notificationPublisher, UserManager userManager, IServiceProvider serviceProvider, IWebHostEnvironment env)
    {
        _notificationPublisher = notificationPublisher;
        _userManager = userManager;

        _serviceProvider = serviceProvider;
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    private void AddItemsFromConfiguration(IConfigurationSection configSection, string key,
    Action<string> itemAdder)
    {
        var items = configSection.GetSection(key).GetChildren().Select(c => c.Value).ToList();
        foreach (var item in items)
        {
            itemAdder(item);
        }
    }


    private async Task SaveApplications(IConfigurationSection child)
    {
        var clientId = child["ClientId"];

        using var scope = _serviceProvider.CreateScope();
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await applicationManager.FindByClientIdAsync(clientId) == null)
        {
            var application = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = child["ClientSecret"],
                ConsentType = child["ConsentType"],
                DisplayName = child["DisplayName"]
            };

            AddItemsFromConfiguration(child, "Scopes",
                (s) => application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + s)
            );

            AddItemsFromConfiguration(child, "Permissions",
                (permission) => application.Permissions.Add(permission)
            );

            AddItemsFromConfiguration(child, "RedirectUris",
                (uri) => application.RedirectUris.Add(new Uri(uri))
            );

            AddItemsFromConfiguration(child, "PostLogoutRedirectUris",
                (uri) => application.PostLogoutRedirectUris.Add(new Uri(uri))
            );

            await applicationManager.CreateAsync(application);
        }
    }

    private async Task SaveScopes(IConfigurationSection child)
    {
        using var scope = _serviceProvider.CreateScope();

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        var scopes = child.GetSection("Scopes").GetChildren().Select(c => c.Value).ToList();

        foreach (var scopeName in scopes)
        {
            await unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (await scopeManager.FindByNameAsync(scopeName) == null)
                {
                    await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        Name = scopeName,
                        DisplayName = scopeName,
                        Resources =
                            {
                                scopeName
                            },
                    });
                }
            });
        }
    }


    public ActionResult Index()
    {

        //if (_appConfiguration["OpenIddict:IsEnabled"] == "true")
        //{
        //    foreach (var child in _appConfiguration.GetSection("OpenIddict:Applications").GetChildren())
        //    {
        //        await SaveScopes(child);
        //        await SaveApplications(child);
        //    }
        //}





        return Redirect("/swagger");
    }

    public ActionResult TestVer()
    {
        return Ok("ok");
    }
 

    /// <summary>
    /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
    /// Don't use this code in production !!!
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<ActionResult> TestNotification(string message = "")
    {
        //var user = await _userManager.GetUserByIdAsync(ADTOSharpSession.UserId.Value);

        var hostAdmin = new UserIdentifier(null, Guid.Parse("00000000-0000-0000-0000-000000000002"));
        await _notificationPublisher.PublishAsync(
            "审核消息",
            new MessageNotificationData(message),
            severity: NotificationSeverity.Info,
            userIds: new[] { hostAdmin }
        );

        await _notificationPublisher.PublishAsync(
            "流程消息",
            new MessageNotificationData(message),
            severity: NotificationSeverity.Warn,
            userIds: new[] { hostAdmin }
        );

        await _notificationPublisher.PublishAsync(
            "订单完结消息",
            new MessageNotificationData(message),
            severity: NotificationSeverity.Success,
            userIds: new[] { hostAdmin }
        );

        await _notificationPublisher.PublishAsync(
            "系统出错",
            new MessageNotificationData(message),
            severity: NotificationSeverity.Error,
            userIds: new[] { hostAdmin }
        );


        // ADTOSharpSession.UserId

        //if (message.IsNullOrEmpty())
        //{
        //    message = "This is a test notification, created at " + Clock.Now;
        //}

        //var defaultTenantAdmin = new UserIdentifier(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"));



        //await _notificationPublisher.PublishAsync(
        //    "App.SimpleMessage",
        //    new MessageNotificationData(message),
        //    severity: NotificationSeverity.Info,
        //    userIds: new[] { defaultTenantAdmin, hostAdmin }
        //);

        return Content("Sent notification: " + message);
    }
}

