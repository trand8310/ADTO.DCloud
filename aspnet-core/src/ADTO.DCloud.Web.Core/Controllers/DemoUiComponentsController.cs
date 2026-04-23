using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTOSharp.IO.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.DemoUiComponents.Dto;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Controllers;

namespace ADTO.DCloud.Web.Controllers;
/// <summary>
/// 配置前端使用,切换主题效果,具体功能还未实现
/// </summary>
[ADTOSharpMvcAuthorize]
public class DemoUiComponentsController : DCloudControllerBase
{
    private readonly IBinaryObjectManager _binaryObjectManager;

    public DemoUiComponentsController(IBinaryObjectManager binaryObjectManager)
    {
        _binaryObjectManager = binaryObjectManager;
    }

    [HttpPost]
    public async Task<JsonResult> UploadFiles()
    {
        try
        {
            var files = Request.Form.Files;

            //Check input
            if (files == null)
            {
                throw new UserFriendlyException(L("File_Empty_Error"));
            }

            List<UploadFileOutput> filesOutput = new List<UploadFileOutput>();

            foreach (var file in files)
            {
                if (file.Length > 1048576) //1MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                byte[] fileBytes;
                using (var stream = file.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var fileObject = new BinaryObject(ADTOSharpSession.TenantId, fileBytes, $"Demo ui, uploaded file {DateTime.UtcNow}");
                await _binaryObjectManager.SaveAsync(fileObject);

                filesOutput.Add(new UploadFileOutput
                {
                    Id = fileObject.Id,
                    FileName = file.FileName
                });
            }

            return Json(new AjaxResponse(filesOutput));
        }
        catch (UserFriendlyException ex)
        {
            return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        }
    }
}
