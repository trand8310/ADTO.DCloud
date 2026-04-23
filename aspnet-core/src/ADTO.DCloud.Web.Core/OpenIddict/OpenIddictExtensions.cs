using OpenIddict.Abstractions;
using System;

namespace ADTO.DCloud.Web.OpenIddict
{
    public static class OpenIddictExtensions
    {
        public static bool IsWechatGrantType(this OpenIddictRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return string.Equals(request.GrantType, "wechat", StringComparison.Ordinal);
        }
    }
}
