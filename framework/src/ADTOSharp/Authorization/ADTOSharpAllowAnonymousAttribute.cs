using System;

namespace ADTOSharp.Authorization
{
    /// <summary>
    /// 允许任何用户访问方法。
    /// 抑制 <see cref="ADTOSharpAuthorizeAttribute"/>
    /// </summary>
    public class ADTOSharpAllowAnonymousAttribute : Attribute, IADTOSharpAllowAnonymousAttribute
    {

    }
}