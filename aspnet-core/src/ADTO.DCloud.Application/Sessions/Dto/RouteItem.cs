using ADTOSharp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Sessions.Dto;

/// <summary>
/// 前端路由
/// </summary>
public class RouteItem
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// 页面地址
    /// </summary>
    public string Component { get; set; }
    /// <summary>
    /// 连接地址
    /// </summary>
    public string Redirect { get; set; }

    public List<RouteItem> Children { get; set; }
}

public class RouteMeta
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public string Hidden { get; set; }
    public string KeepAlive { get; set; }
    public string AlwaysShow { get; set; }
    public Dictionary<string, string> Params { get; set; }
}

