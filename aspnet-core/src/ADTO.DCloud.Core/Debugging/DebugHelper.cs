namespace ADTO.DCloud.Debugging;

/// <summary>
/// 利用宏,判定当前版本是否处于调试模式
/// </summary>
public static class DebugHelper
{
    /// <summary>
    /// 是否调试模式
    /// </summary>
    public static bool IsDebug
    {
        get
        {
#pragma warning disable
#if DEBUG
            return true;
#endif
            return false;
#pragma warning restore
        }
    }
}
