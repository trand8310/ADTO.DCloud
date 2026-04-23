using ADTOSharp.Reflection.Extensions;
using System;
using System.IO;

namespace ADTO.DCloud;

/// <summary>
/// 应用程序版本
/// </summary>
public class AppVersionHelper
{
    /// <summary>
    /// 系统版本
    /// It's also shown in the web page.
    /// </summary>
    public const string Version = "1.0.0";

    /// <summary>
    /// 获取应用程序的发布（上次构建）日期。
    /// </summary>
    public static DateTime ReleaseDate => LzyReleaseDate.Value;

    private static readonly Lazy<DateTime> LzyReleaseDate = new Lazy<DateTime>(() => new FileInfo(typeof(AppVersionHelper).GetAssembly().Location).LastWriteTime);
}
