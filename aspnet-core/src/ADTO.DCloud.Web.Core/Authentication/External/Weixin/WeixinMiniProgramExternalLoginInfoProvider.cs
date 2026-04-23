using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authentication.External.Weixin;

/// <summary>
/// 微信小程序登录服务提供
/// </summary>
public class WeixinMiniProgramExternalLoginInfoProvider : IExternalLoginInfoProvider
{
    public string Name => "WeixinMiniProgram";
    protected string AppId { get; set; }
    protected string AppSecret { get; set; }
    protected ExternalLoginProviderInfo ExternalLoginProviderInfo { get; set; }
    public WeixinMiniProgramExternalLoginInfoProvider(string appId, string appSecret)
    {
        AppId = appId;
        AppSecret = appSecret;
        ExternalLoginProviderInfo = new ExternalLoginProviderInfo(Name, AppId, AppSecret, typeof(WeixinMiniProgramAuthProviderApi));
    }
    public virtual ExternalLoginProviderInfo GetExternalLoginInfo()
    {
        return ExternalLoginProviderInfo;
    }
}
