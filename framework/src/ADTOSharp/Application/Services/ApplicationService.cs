using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Features;
using ADTOSharp.Authorization;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Application.Services
{
    /// <summary>
    /// 该类可以用作应用程序服务的基类。
    /// </summary>
    public abstract class ApplicationService : ADTOSharpServiceBase, IApplicationService, IAvoidDuplicateCrossCuttingConcerns
    {
        public static string[] CommonPostfixes = { "AppService", "ApplicationService" };

        /// <summary>
        /// 获取当前会话信息.
        /// </summary>
        public IADTOSharpSession ADTOSharpSession { get; set; }

        /// <summary>
        /// 权限管理.
        /// </summary>
        public IPermissionManager PermissionManager { protected get; set; }

        /// <summary>
        /// 权限检查.
        /// </summary>
        public IPermissionChecker PermissionChecker { protected get; set; }

        /// <summary>
        /// 特性管理
        /// </summary>
        public IFeatureManager FeatureManager { protected get; set; }

        /// <summary>
        /// 特性检查.
        /// </summary>
        public IFeatureChecker FeatureChecker { protected get; set; }

        /// <summary>
        /// 获取应用的横切关注点
        /// </summary>
        public List<string> AppliedCrossCuttingConcerns { get; } = new List<string>();

        /// <summary>
        /// 构造函数.
        /// </summary>
        protected ApplicationService()
        {
            ADTOSharpSession = NullADTOSharpSession.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
        }

        /// <summary>
        /// 检查当前用户是否被授予权限(异步)。
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        protected virtual Task<bool> IsGrantedAsync(string permissionName)
        {
            return PermissionChecker.IsGrantedAsync(permissionName);
        }

        /// <summary>
        /// 检查当前用户是否被授予权限
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        protected virtual bool IsGranted(string permissionName)
        {
            return PermissionChecker.IsGranted(permissionName);
        }

        /// <summary>
        /// 检查是否为当前租户启用了给定的特性(异步)
        /// </summary>
        /// <param name="featureName">特性名称</param>
        /// <returns></returns>
        protected virtual Task<bool> IsEnabledAsync(string featureName)
        {
            return FeatureChecker.IsEnabledAsync(featureName);
        }

        /// <summary>
        /// 检查是否为当前租户启用了给定的特性
        /// </summary>
        /// <param name="featureName">特性名称</param>
        /// <returns></returns>
        protected virtual bool IsEnabled(string featureName)
        {
            return FeatureChecker.IsEnabled(featureName);
        }
    }
}
