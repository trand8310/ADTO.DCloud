using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// 请求授权
    /// </summary>
    public class RequestPermission
    {
        /// <summary>
        /// 获取请求密钥
        /// </summary>
        public static string GetRequestToken
        {
            get
            {
                if (IocManager.Instance.IocContainer != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
                {
                    var accessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
                    if (accessor.HttpContext != null)
                    {
                        string token = string.Empty;
                        if (!accessor.HttpContext.Request.Headers["token"].IsEmpty())
                        {
                            token = accessor.HttpContext.Request.Headers["token"].ToString();
                        }
                        //若是开启密级，则无法使用除header以外的token传参----ADD BY SSY 20230419
                        //if (!ConfigHelper.GetConfig().IsSecretLevel)
                        //{
                        if (string.IsNullOrEmpty(token))
                        {
                            try
                            {
                                token = accessor.HttpContext.Request.Query["token"];
                            }
                            catch (Exception)
                            {
                                token = string.Empty;
                            }
                        }

                        if (string.IsNullOrEmpty(token))
                        {
                            try
                            {
                                if (accessor.HttpContext.Request.HasFormContentType)
                                {
                                    token = accessor.HttpContext.Request.Form["token"];
                                }
                                else
                                {
                                    token = string.Empty;
                                }
                            }
                            catch (Exception)
                            {
                                token = string.Empty;
                            }
                        }
                        return token;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
