using ADTO.DCloud.Controllers;
using ADTO.DCloud.Media.UploadFiles;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Web.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UEditorController : DCloudControllerBase
    {
        #region Fields
        private readonly IUploadFileAppService _uploadFileAppService;

        #endregion

        #region Ctor

        public UEditorController(
            IUploadFileAppService uploadFileAppService)
        {
            _uploadFileAppService = uploadFileAppService;
        } 
        #endregion 

        #region Methods 
        [DontWrapResult]
        [HttpPost, HttpGet]
        [IgnoreAntiforgeryToken]
        public virtual void ProcessRequest()
        {
            ProcessRequestAsync().Wait();
        }

        #endregion

        #region Utilities 
        [IgnoreAntiforgeryToken]
        protected virtual async Task ProcessRequestAsync()
        {
            var action = "DIRLIST";
            try
            {
                action = HttpContext.Request.Query["action"];

                switch (action.ToUpper())
                {
                    case "CONFIG":
                        var callback = HttpContext.Request.Query["callback"];
                        var configText = _uploadFileAppService.GetUEditorConfiguration();
                        HttpContext.Response.ContentType = "application/javascript; charset=utf-8";
                        await HttpContext.Response.WriteAsync($"{callback}({configText})", Encoding.UTF8);
                        break;
                    case "UPLOADIMAGE":
                        HttpContext.Response.ContentType = "application/javascript; charset=utf-8";
                        await _uploadFileAppService.UploadFilesUEditAsync();//"/images/uploaded/"
                        break;
                    //case "LISTIMAGE":
                    //    await _ueditorService.GetFilesAsync("/images/uploaded/", "image");
                    //    break;
                    //case "DIRLIST":
                    //    await _ueditorService.GetDirectoriesAsync(HttpContext.Request.Query["type"]);
                    //    break;
                    //case "FILESLIST":
                    //    await _ueditorService.GetFilesAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["type"]);
                    //    break;
                    //case "COPYDIR":
                    //    await _ueditorService.CopyDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "COPYFILE":
                    //    await _ueditorService.CopyFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "CREATEDIR":
                    //    await _ueditorService.CreateDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "DELETEDIR":
                    //    await _ueditorService.DeleteDirectoryAsync(HttpContext.Request.Query["d"]);
                    //    break;
                    //case "DELETEFILE":
                    //    await _ueditorService.DeleteFileAsync(HttpContext.Request.Query["f"]);
                    //    break;
                    //case "DOWNLOAD":
                    //    await _ueditorService.DownloadFileAsync(HttpContext.Request.Query["f"]);
                    //    break;
                    //case "DOWNLOADDIR":
                    //    await _ueditorService.DownloadDirectoryAsync(HttpContext.Request.Query["d"]);
                    //    break;
                    //case "MOVEDIR":
                    //    await _ueditorService.MoveDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "MOVEFILE":
                    //    await _ueditorService.MoveFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "RENAMEDIR":
                    //    await _ueditorService.RenameDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "RENAMEFILE":
                    //    await _ueditorService.RenameFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                    //    break;
                    //case "GENERATETHUMB":
                    //    _ueditorService.CreateImageThumbnail(HttpContext.Request.Query["f"]);
                    //    break;
                    case "UPLOAD":
                        await _uploadFileAppService.UploadFilesUEditAsync();//HttpContext.Request.Form["d"]
                        break;
                    //上传附件
                    case "UPLOADFILE":
                        //await _uploadFileAppService.UploadFilesOssAsync("/images/uploaded/file", true);
                        await _uploadFileAppService.UploadFilesUEditAsync();
                        break;
                    //查看附件
                    case "LISTFILE":
                        //await _uploadFileAppService.GetFilesAsync("/images/uploaded/file", "#");
                        break;
                    default:
                        await HttpContext.Response.WriteAsync(_uploadFileAppService.GetErrorResponse("This action is not implemented."));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (action == "UPLOAD" && !_uploadFileAppService.IsAjaxRequest())
                    await HttpContext.Response.WriteAsync($"<script>parent.fileUploaded({_uploadFileAppService.GetErrorResponse(L("E_UploadNoFiles"))});</script>");
                else
                    await HttpContext.Response.WriteAsync(_uploadFileAppService.GetErrorResponse(ex.Message));
            }
        }
        #endregion
    }
}
