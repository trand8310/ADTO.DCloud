using ADTOSharp;

namespace ADTO.DCloud
{
    /// <summary>
    /// 这个类可以用作这个应用程序中服务的基类。
    /// 它有一些有用的属性注入对象和一些大多数服务可能需要的基本方法。
    /// 它适用于非域或应用服务类。
    /// 对于域服务继承<见cref="ADTOSharpServiceBase"/>。
    /// 对于应用程序服务继承ADTOSharpServiceBase。
    /// </summary>
    public abstract class DCloudServiceBase : ADTOSharpServiceBase
    {
        protected DCloudServiceBase()
        {
            LocalizationSourceName = DCloudConsts.LocalizationSourceName;
        }
    }
}
