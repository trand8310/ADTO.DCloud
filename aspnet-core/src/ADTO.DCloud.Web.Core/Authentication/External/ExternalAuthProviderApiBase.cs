using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ADTOSharp.Dependency;
using Newtonsoft.Json.Linq;

namespace ADTO.DCloud.Authentication.External;

public abstract class ExternalAuthProviderApiBase : IExternalAuthProviderApi, ITransientDependency
{
    public ExternalLoginProviderInfo ProviderInfo { get; set; }

    public void Initialize(ExternalLoginProviderInfo providerInfo)
    {
        ProviderInfo = providerInfo;
    }

    public async Task<bool> IsValidUser(string userId, string accessCode)
    {
        var userInfo = await GetUserInfo(accessCode);
        return userInfo.ProviderKey == userId;
    }

    public abstract Task<ExternalAuthUserInfo> GetUserInfo(string accessCode);

    protected virtual void FillClaimsFromJObject(ExternalAuthUserInfo userInfo, JObject payload)
    {
        List<ClaimKeyValue> list = new List<ClaimKeyValue>();
        foreach (KeyValuePair<string, JToken> item in payload)
        {
            list.Add(new ClaimKeyValue(item.Key, ((object)item.Value)?.ToString()));
        }
        userInfo.Claims = list;
    }
}
