using ADTO.DCloud.Url;
using ADTOSharp.MultiTenancy;

namespace ADTO.DCloud.Web.Url;

/// <summary>
/// 前端需要存在所列URL后缀,用于邮件内容中的,地址操作.
/// </summary>
public class VueAppUrlService : AppUrlServiceBase
{
    /// <summary>
    /// 邮件确认
    /// </summary>
    public override string EmailActivationRoute => "account/confirm-email";
    /// <summary>
    /// 修改邮件地址
    /// </summary>
    public override string EmailChangeRequestRoute => "account/change-email";
    /// <summary>
    /// 重置密码
    /// </summary>
    public override string PasswordResetRoute => "account/reset-password";

    public VueAppUrlService(
            IWebUrlService webUrlService,
            ITenantCache tenantCache
        ) : base(
            webUrlService,
            tenantCache
        )
    {

    }
}