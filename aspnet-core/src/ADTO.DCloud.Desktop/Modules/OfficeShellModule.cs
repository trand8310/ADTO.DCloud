using Prism.Ioc;
using Prism.Modularity;

namespace ADTO.DCloud.Desktop.Modules;

public sealed class OfficeShellModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 模块入口预留：后续审批、任务、消息、报表等桌面模块可在这里继续注册服务与页面。
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        // 模块初始化预留：可在这里启动模块级缓存、消息订阅或导航菜单扩展。
    }
}
