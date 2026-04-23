using System;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTOSharp.IO.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.Chat;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Controllers;

namespace ADTO.DCloud.Web.Controllers;

/// <summary>
/// 消息服务控制器基类
/// </summary>
public class ChatControllerBase : DCloudControllerBase
{
    protected readonly IBinaryObjectManager BinaryObjectManager;
    protected readonly IChatMessageManager ChatMessageManager;

    public ChatControllerBase(IBinaryObjectManager binaryObjectManager, IChatMessageManager chatMessageManager)
    {
        BinaryObjectManager = binaryObjectManager;
        ChatMessageManager = chatMessageManager;
    }

    /// <summary>
    /// 上传文件,
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ADTOSharpMvcAuthorize]
    public async Task<JsonResult> UploadFile()
    {
        try
        {
             
            var file = Request.Form.Files.First();

            //Check input
            if (file == null)
            {
                throw new UserFriendlyException(L("File_Empty_Error"));
            }

            if (file.Length > 10000000) //10MB
            {
                throw new UserFriendlyException(L("File_SizeLimit_Error"));
            }

            byte[] fileBytes;
            using (var stream = file.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            var fileObject = new BinaryObject(null, fileBytes, $"File uploaded from chat by {ADTOSharpSession.UserId}, File name: {file.FileName} {DateTime.UtcNow}");
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                await BinaryObjectManager.SaveAsync(fileObject);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return Json(new AjaxResponse(new
            {
                id = fileObject.Id,
                name = file.FileName,
                contentType = file.ContentType
            }));
        }
        catch (UserFriendlyException ex)
        {
            return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        }
    }
}
