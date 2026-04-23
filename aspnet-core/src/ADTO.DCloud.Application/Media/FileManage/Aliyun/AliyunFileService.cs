using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using AlibabaCloud.SDK.Sts20150401.Models;
using Aliyun.OSS;
using Tea;
using ADTOSharp.Web.Models;
using System.IO;
using ADTO.DCloud.Storage;
using System.Threading.Tasks;
using Aliyun.OSS.Common;
using ADTOSharp.Runtime.Caching;
using Microsoft.AspNetCore.Http;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Media.FileManage.Aliyun
{
    /// <summary>
    /// 阿里云
    /// </summary>
    public class AliyunFileService : IAliyunFileService, IFileManageService
    {
        private static string CacheName = "AliyunPrivateFileCache";
        #region
        private static IConfiguration _configuration;
        private readonly ICacheManager _cacheManager;
        public AliyunFileService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="accessKeyId">必填，您的 AccessKey ID</param>
        /// <param name="accessKeySecret">必填，您的 AccessKey Secret</param>
        /// <param name="endpoint"> // Endpoint 请参考 https://api.aliyun.com/product/Sts</param>
        /// <returns></returns>
        public static AlibabaCloud.SDK.Sts20150401.Client CreateClient(string accessKeyId, string accessKeySecret, string endpoint)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = accessKeyId,
                AccessKeySecret = accessKeySecret,
            };
            //config.Endpoint = "sts.cn-hangzhou.aliyuncs.com";
            config.Endpoint = endpoint;
            return new AlibabaCloud.SDK.Sts20150401.Client(config);
        }

        /// <summary>
        /// AssumeRole获取临时身份凭证
        /// </summary>
        public AssumeRoleResponseBody GetAliyunTSTAccessKey()
        {
            AlibabaCloud.SDK.Sts20150401.Client client = CreateClient(_configuration["Aliyun:accessKeyId"], _configuration["Aliyun:accessKeySecret"], _configuration["Aliyun:stsEndpoint"]);
            AssumeRoleRequest assumeRoleRequest = new AssumeRoleRequest
            {
                //RoleArn = "acs:ram::1240797526173107:role/ramossaccess",
                RoleArn = _configuration["Aliyun:assumeRoleArn"],
                RoleSessionName = _configuration["Aliyun:assumeRoleSessionName"],
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var vresult = client.AssumeRoleWithOptions(assumeRoleRequest, runtime);
                return vresult.Body;
            }
            catch (TeaException error)
            {
                // 错误 message
                Console.WriteLine(error.Message);
                // 诊断地址
                Console.WriteLine(error.Data["Recommend"]);
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 错误 message
                Console.WriteLine(error.Message);
                // 诊断地址
                Console.WriteLine(error.Data["Recommend"]);
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            return null;
        }
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 简单上传文件，并返回完整地址 .net sdk方式上传文件
        /// </summary>
        /// <param name="fileInfo">内存中存储的图片对象</param>
        /// <returns></returns>
        public AjaxResponse<object> PutFileAliyunNormal(TempFileInfo fileInfo)
        {
            AjaxResponse<object> response = new AjaxResponse<object>();

            var endpoint = _configuration["Aliyun:endpoint"];
            var bucketName = _configuration["Aliyun:bucketName"];

            //获取临时凭证请求阿里云
            var vAssumeRole = GetAliyunTSTAccessKey();
            // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.FileName);
            var fileExtension = Path.GetExtension(fileInfo.FileName);
            var dirObjectName = _configuration["Aliyun:fileDir"] + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName + "_" + GetTimeStamp() + fileExtension;
            // 创建OssClient实例。
            var client = new OssClient(endpoint, vAssumeRole.Credentials.AccessKeyId, vAssumeRole.Credentials.AccessKeySecret, vAssumeRole.Credentials.SecurityToken);
            try
            {
                response.Success = true;

                Stream stream = new MemoryStream(fileInfo.File);
                // 上传文件
                var result = client.PutObject(bucketName, dirObjectName, stream);
                response.Result = dirObjectName;
            }
            catch (Exception ex)
            {
                response.Error = new ErrorInfo() { Message = ex.Message };
            }
            return response;
        }

        /// <summary>
        /// 简单上传文件，并返回完整地址 .net sdk方式上传文件
        /// </summary>
        /// <param name="formFile">前端传过来的文件对象</param>
        /// <returns></returns>
        public AjaxResponse<object> PutFileAliyunNormal(IFormFile formFile)
        {
            AjaxResponse<object> response = new AjaxResponse<object>();

            var endpoint = _configuration["Aliyun:endpoint"];
            var bucketName = _configuration["Aliyun:bucketName"];

            //获取临时凭证请求阿里云
            var vAssumeRole = GetAliyunTSTAccessKey();
            // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var fileExtension = Path.GetExtension(formFile.FileName);
            var dirObjectName = _configuration["Aliyun:fileDir"] + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName + "_" + GetTimeStamp() + fileExtension;
            // 创建OssClient实例。
            var client = new OssClient(endpoint, vAssumeRole.Credentials.AccessKeyId, vAssumeRole.Credentials.AccessKeySecret, vAssumeRole.Credentials.SecurityToken);
            try
            {
                response.Success = true;

                var stream = formFile.OpenReadStream();
                // 上传文件
                var result = client.PutObject(bucketName, dirObjectName, stream);
                response.Result = dirObjectName;
            }
            catch (Exception ex)
            {
                response.Error = new ErrorInfo() { Message = ex.Message };
            }
            return response;
        }

        /// <summary>
        /// 保存文件，内存方式
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public async Task<AjaxResponse<object>> SaveFilesAsync(TempFileInfo fileInfo)
        {
            return PutFileAliyunNormal(fileInfo);
        }

        /// <summary>
        /// 保存文件(非内存方式)
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public async Task<AjaxResponse<object>> SaveFilesAsync(IFormFile fileInfo)
        {
            return PutFileAliyunNormal(fileInfo);
        }

        /// <summary>
        /// 获取公共文件访问地址
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public AjaxResponse<object> GetPublicFileUrl(string objectName)
        {
            AjaxResponse<object> response = new AjaxResponse<object>();
            var cdnDomainName = _configuration["Aliyun:cdnDomainName"];
            //完整地址
            response.Result = cdnDomainName + objectName;
            return response;
        }

        /// <summary>
        ///  获取私有文件访问地址(临时访问权限的预签名URL)
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public AjaxResponse<object> GetPrivateFileUrl(string objectName)
        {
            var cache = _cacheManager.GetCache(CacheName).Get("35550F94-A17B-4A1C-4CAF-08DD5610B5BE", p => p).ToString();


            // var provider = _cacheManager.GetCache("ProviderCache").Get("Provider", cache => cache).ToString();

            //  _cacheManager.GetCache(objectName).Set(objectName, "缓存值", TimeSpan.FromMinutes(8));


            AjaxResponse<object> response = new AjaxResponse<object>();

            var endpoint = _configuration["Aliyun:endpoint"];
            var accessKeyId = _configuration["Aliyun:accessKeyId"];
            var accessKeySecret = _configuration["Aliyun:accessKeySecret"];
            var bucketName = _configuration["Aliyun:bucketName"];

            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                // URL的有效期，改成AddHours 就不行
                DateTime expiration = DateTime.Now.AddMinutes(8);
                // 生成带签名的URL（不同文件对象，不能共用返回签名）
                Uri uri = client.GeneratePresignedUri(bucketName, objectName, expiration);
                response.Result = uri;

                // 将 URI 存储到缓存中
                _cacheManager.GetCache(objectName).Set(objectName, uri.ToString(), TimeSpan.FromMinutes(8)); // 缓存有效期与 URL 有效期一致

            }
            catch (OssException ex)
            {
                response.Error.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Error.Message = ex.Message;
            }
            return response;
        }



        public JsonResultModel<PutObjectResult> SaveFile(TempFileInfo fileInfo)
        {
            var result = new JsonResultModel<PutObjectResult>();
            var endpoint = _configuration["Aliyun:endpoint"];
            var bucketName = _configuration["Aliyun:bucketName"];
            //获取临时凭证请求阿里云
            var vAssumeRole = GetAliyunTSTAccessKey();
            // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.FileName);
            var fileExtension = Path.GetExtension(fileInfo.FileName);
            var dirObjectName = _configuration["Aliyun:fileDir"] + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName + "_" + GetTimeStamp() + fileExtension;
            // 创建OssClient实例。
            var client = new OssClient(endpoint, vAssumeRole.Credentials.AccessKeyId, vAssumeRole.Credentials.AccessKeySecret, vAssumeRole.Credentials.SecurityToken);
            try
            {
                Stream stream = new MemoryStream(fileInfo.File);
                var response = client.PutObject(bucketName, dirObjectName, stream);
                return result.SuccessInfo("操作成功", data: response, new Dictionary<string, object>() {
                    {"dirObjectName",dirObjectName }
                });
            }
            catch (Exception ex)
            {
                return result.ErrorInfo(ex.Message);
            }
        }


        public JsonResultModel<PutObjectResult> SaveFile(IFormFile formFile)
        {
            var result = new JsonResultModel<PutObjectResult>();
            var endpoint = _configuration["Aliyun:endpoint"];
            var bucketName = _configuration["Aliyun:bucketName"];
            //获取临时凭证请求阿里云
            var vAssumeRole = GetAliyunTSTAccessKey();
            // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var fileExtension = Path.GetExtension(formFile.FileName);
            var dirObjectName = _configuration["Aliyun:fileDir"] + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName + "_" + GetTimeStamp() + fileExtension;
            // 创建OssClient实例。
            var client = new OssClient(endpoint, vAssumeRole.Credentials.AccessKeyId, vAssumeRole.Credentials.AccessKeySecret, vAssumeRole.Credentials.SecurityToken);
            try
            {
                Stream stream = formFile.OpenReadStream();
                var response = client.PutObject(bucketName, dirObjectName, stream);
                return result.SuccessInfo("操作成功", data: response, new Dictionary<string, object>() {
                    {"dirObjectName",dirObjectName },
                    {"fileName",fileName }
                });
            }
            catch (Exception ex)
            {
                return result.ErrorInfo(ex.Message);
            }
        }

    }
}

