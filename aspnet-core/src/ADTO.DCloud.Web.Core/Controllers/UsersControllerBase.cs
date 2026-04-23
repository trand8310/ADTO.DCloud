using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTOSharp.IO.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Storage;
using ADTOSharp.BackgroundJobs;
using ADTO.DCloud.Authorization;
using ADTOSharp.Runtime.Session;
using ADTO.DCloud.Authorization.Users.Importing;
using ADTO.DCloud.Controllers;
using ADTOSharp.AspNetCore.Mvc.Authorization;

namespace ADTO.DCloud.Web.Controllers;

/// <summary>
/// 用户管理控制抽象基类,主要管理用户的文件及一些任务排程
/// </summary>
public abstract class UsersControllerBase : DCloudControllerBase
{
    protected readonly IBinaryObjectManager BinaryObjectManager;
    protected readonly IBackgroundJobManager BackgroundJobManager;

    protected UsersControllerBase(
        IBinaryObjectManager binaryObjectManager,
        IBackgroundJobManager backgroundJobManager)
    {
        BinaryObjectManager = binaryObjectManager;
        BackgroundJobManager = backgroundJobManager;
    }

    [HttpPost]
    [ADTOSharpMvcAuthorize(PermissionNames.Pages_Administration_Users_Create)]
    public async Task<JsonResult> ImportFromExcel()
    {
        try
        {
            var file = Request.Form.Files.First();

            if (file == null)
            {
                throw new UserFriendlyException(L("File_Empty_Error"));
            }

            if (file.Length > 1048576 * 100) //100 MB
            {
                throw new UserFriendlyException(L("File_SizeLimit_Error"));
            }

            byte[] fileBytes;
            using (var stream = file.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            var tenantId = ADTOSharpSession.TenantId;
            var fileObject = new BinaryObject(tenantId, fileBytes, $"{DateTime.UtcNow} import from excel file.");

            await BinaryObjectManager.SaveAsync(fileObject);

            await BackgroundJobManager.EnqueueAsync<ImportUsersToExcelJob, ImportUsersFromExcelJobArgs>(new ImportUsersFromExcelJobArgs
            {
                TenantId = tenantId,
                BinaryObjectId = fileObject.Id,
                User = ADTOSharpSession.ToUserIdentifier()
            });

            return Json(new AjaxResponse(new { }));
        }
        catch (UserFriendlyException ex)
        {
            return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        }
    }
}

