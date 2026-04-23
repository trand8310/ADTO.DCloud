using System;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.Notifications;
using ADTO.DCloud.Controllers;

namespace ADTO.DCloud.Web.Controllers;

/// <summary>
/// 浏览器缓存清理控制器
/// </summary>
public class BrowserCacheCleanerController : DCloudControllerBase
{
    private readonly INotificationAppService _notificationAppService;

    public BrowserCacheCleanerController(INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    public async Task<IActionResult> Clear()
    {
        var result = await _notificationAppService.SetAllAvailableVersionNotificationAsRead();

        HttpContext.Response.Headers.Append("Clear-Site-Data", "\"cache\"");

        return Json(new { Result = result });
    }
}
