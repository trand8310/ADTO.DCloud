using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// http方法
    /// </summary>
    public class HttpMethods
    {
        //
        // 摘要:
        //     创建HttpClient
        public static HttpClient CreateHttpClient(string url, IDictionary<string, string> cookies = null)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            Uri uri = new Uri(url);
            if (cookies != null)
            {
                foreach (string key in cookies.Keys)
                {
                    string cookieHeader = key + "=" + cookies[key];
                    httpClientHandler.CookieContainer.SetCookies(uri, cookieHeader);
                }
            }

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>)Delegate.Combine(httpClientHandler.ServerCertificateCustomValidationCallback, (Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>)((HttpRequestMessage sender, X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
                return new HttpClient(httpClientHandler);
            }

            return new HttpClient(httpClientHandler);
        }
        //
        // 摘要:
        //     post 请求
        //
        // 参数:
        //   url:
        //     请求地址
        //
        //   jsonData:
        //     请求参数
        public static Task<string> Post(string url, string jsonData)
        {
            HttpClient httpClient = CreateHttpClient(url);
            StringContent stringContent = new StringContent(jsonData);
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return httpClient.PostAsync(url, stringContent).Result.Content.ReadAsStringAsync();
        }
    }
}
