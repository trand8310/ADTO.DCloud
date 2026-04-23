using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using ADTOSharp.Web.Models;
using AlibabaCloud.SDK.Sts20150401.Models;
using Aliyun.OSS;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ADTO.DCloud.Media.FileManage.Aliyun
{
    /// <summary>
    /// OSS阿里云操作
    /// </summary>
    public interface IAliyunFileService
    {
        /// <summary>
        /// STS获取临时身份凭证
        /// </summary>
        AssumeRoleResponseBody GetAliyunTSTAccessKey();

        /// <summary>
        /// 简单上传文件，并返回完整地址 .net sdk方式上传文件
        /// </summary>
        /// <param name="fileInfo">内存中存储的图片对象</param>
        /// <returns></returns>
        AjaxResponse<object> PutFileAliyunNormal(TempFileInfo fileInfo);


        JsonResultModel<PutObjectResult> SaveFile(TempFileInfo fileInfo);
        JsonResultModel<PutObjectResult> SaveFile(IFormFile formFile);
    }
}
