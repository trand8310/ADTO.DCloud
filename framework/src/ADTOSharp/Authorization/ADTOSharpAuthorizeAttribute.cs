using System;
using ADTOSharp.Application.Services;

namespace ADTOSharp.Authorization
{
    /// <summary>
    /// 此属性用于应用程序服务的方法 (这些类需要实现 <see cref="IApplicationService"/>)
    /// 使该方法仅供授权用户使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ADTOSharpAuthorizeAttribute : Attribute, IADTOSharpAuthorizeAttribute
    {
        /// <summary>
        /// 授权的权限列表
        /// </summary>
        public string[] Permissions { get; }

        /// <summary>
        /// 如果此属性设置为true，则所有的 <see cref="Permissions"/> 必须被授权.
        /// 如果是假的，至少有一个 <see cref="Permissions"/> 必须被授权.
        /// Default: false.
        /// </summary>
        public bool RequireAllPermissions { get; set; }

        /// <summary>
        ///  创建一个新的 <see cref="ADTOSharpAuthorizeAttribute"/> 实例.
        /// </summary>
        /// <param name="permissions">要授权的权限列表</param>
        public ADTOSharpAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
    }
}
