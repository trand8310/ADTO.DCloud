using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using ADTOSharp.Web.Models;
using Aliyun.OSS;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ADTO.DCloud.Media.FileManage
{
    public interface IFileManageService
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        Task<AjaxResponse<object>> SaveFilesAsync(TempFileInfo fileInfo);

        /// <summary>
        /// 保存文件(非内存方式)
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        Task<AjaxResponse<object>> SaveFilesAsync(IFormFile fileInfo);

        AjaxResponse<object> GetPublicFileUrl(string objectName);

        /// <summary>
        ///  获取私有文件访问地址
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        AjaxResponse<object> GetPrivateFileUrl(string objectName);




    }
}
