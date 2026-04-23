using ADTO.DCloud.Configuration;
using ADTOSharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authentication.External.Weixin;

/// <summary>
/// 微信小程序,接口操作
/// </summary>
public class WeixinMiniProgramAuthProviderApi : ExternalAuthProviderApiBase
{
    public const string Name = "WeixinMiniProgram";
    private readonly IExternalAuthConfiguration _externalAuthConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfigurationRoot _appConfiguration;
    private readonly IWebHostEnvironment _hostingEnvironment;
    public WeixinMiniProgramAuthProviderApi(IWebHostEnvironment env, IExternalAuthConfiguration externalAuthConfiguration, IHttpClientFactory httpClientFactory)
    {
        _appConfiguration = env.GetAppConfiguration();
        _externalAuthConfiguration = externalAuthConfiguration;
        _httpClientFactory = httpClientFactory;
    }
    private async Task<Jscode2SessionResult> Jscode2Session(string appId, string accessCode)
    {
        var provider = this._externalAuthConfiguration.ExternalLoginInfoProviders.FirstOrDefault(p => p.GetExternalLoginInfo().ClientId.Equals(appId));
        var providerInfo = provider.GetExternalLoginInfo();
        var appid = providerInfo.ClientId;
        var secret = providerInfo.ClientSecret;
        try
        {
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={secret}&js_code={accessCode}&grant_type=authorization_code";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Jscode2SessionResult>(result);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new ADTOSharpException("Jscode2Session:" + ex.Message);
        }
    }


    public async override Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
    {
        //var userData = accessCode;// (JObject)JsonConvert.DeserializeObject(accessCode);
        var appId = _appConfiguration["Authentication:WeixinMiniProgram:AppId"];// userData["appId"].ToString();
        //var code = _appConfiguration["Authentication:WeixinMiniProgram:AppSecret"];// userData["accessCode"].ToString();
        var sessionResult = await Jscode2Session(appId, accessCode);
        var user = sessionResult == null ? new ExternalAuthUserInfo() : new ExternalAuthUserInfo
        {
            Name = "微信用户",//这里需要的是昵称,但现在调用这个接口时获取不到,需要二次获取,所以,现在统一为这个值,如果用户有自行修改,这里的固定值,不会影响后面的操作
            Provider = WeixinMiniProgramAuthProviderApi.Name,
            ProviderKey = sessionResult.unionid ?? sessionResult.openid,
        };
        return user;
    }
}
