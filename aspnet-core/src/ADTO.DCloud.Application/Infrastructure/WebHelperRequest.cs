using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// 第三方接口请求
    /// </summary>
    public class WebHelperRequest
    {
        #region HttpWebRequest(请求网络资源)

        /// <summary>
        /// 请求网络资源,返回响应的文本
        /// </summary>
        /// <param name="url">网络资源地址</param>
        public static string HttpWebRequest(string url)
        {
            return HttpWebRequest(url, string.Empty, Encoding.GetEncoding("utf-8"));
        }

        /// <summary>
        /// 请求网络资源,返回响应的文本
        /// </summary>
        /// <param name="url">网络资源Url地址</param>
        /// <param name="parameters">提交的参数,格式：参数1=参数值1&amp;参数2=参数值2</param>
        public static string HttpWebRequest(string url, string parameters)
        {
            return HttpWebRequest(url, parameters, Encoding.GetEncoding("utf-8"), true);
        }

        /// <summary>
        /// 请求网络资源,返回响应的文本
        /// </summary>
        /// <param name="url">网络资源地址</param>
        /// <param name="parameters">提交的参数,格式：参数1=参数值1&amp;参数2=参数值2</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isPost">是否Post提交</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="cookie">Cookie容器</param>
        /// <param name="timeout">超时时间</param>
        public static string HttpWebRequest(string url, string parameters, Encoding encoding, bool isPost = false,
             string contentType = "application/json", CookieContainer cookie = null, int timeout = 120000)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout;
            request.CookieContainer = cookie;
            if (isPost)
            {
                byte[] postData = encoding.GetBytes(parameters);
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = postData.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            var response = (HttpWebResponse)request.GetResponse();
            string result;
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                    return string.Empty;
                using (var reader = new StreamReader(stream, encoding))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        #endregion
    }
}
