using ADTOSharp.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{

    public class HttpClientHelper
    {
        private readonly HttpClient _client;
        public HttpClientHelper(HttpClient client)
        {
            _client = client;
        }
        /// <summary>
        /// 请求API地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task GetRequest(string url)
        {
            // 创建 HttpClient 实例
            using (var client = new HttpClient())
            {
                // 设置基本的请求头，假如有必要
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                try
                {
                    // 调用 API 接口
                    var response = await client.GetAsync(url);
                    // 确保响应成功
                    if (response.IsSuccessStatusCode)
                    {
                        // 获取响应内容
                        var content = await response.Content.ReadAsStringAsync();
                        // 输出响应内容
                        Console.WriteLine("Response: " + content);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException($"{ex.Message}");
                }
            }
        }
        /// <summary>
        /// post请求
        /// </summary>
        /// <returns></returns>
        public async Task PostRequest(string url,object postData)
        {
            // 将数据转换为 JSON 字符串
            var jsonContent = JsonConvert.SerializeObject(postData);
            // 创建 StringContent，设置内容类型为 JSON
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            try
            {
                // 发起 POST 请求
                var response = await _client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"{ex.Message}");
            }
        }

    }
}
