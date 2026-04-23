using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ADTOSharp.Authorization;
using ADTO.DCloud.Controllers;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using ADTOSharp.IO.Extensions;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Configuration;
using ADTOSharp;

namespace ADTO.DCloud.Web.Controllers;


/// <summary>
/// 文件上传
/// </summary>
[ADTOSharpAuthorize]
[Route("api/[controller]/[action]")]
public class UploadController : DCloudControllerBase
{
    private readonly ITempFileCacheManager _tempFileCacheManager;
    private readonly IGuidGenerator _guidGenerator;
    public UploadController(ITempFileCacheManager tempFileCacheManager,IGuidGenerator guidGenerator)
    {
        _tempFileCacheManager = tempFileCacheManager;
        _guidGenerator = guidGenerator;
    }

    #region Methods
    /// <summary>
    /// 上传文件到缓冲区,并返回文件的TOKENID,
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public ActionResult<List<FileDto>> UploadFile()
    {
        var result = new List<FileDto>();
        foreach (var formFile in Request.Form.Files)
        {
            var fileName = formFile.FileName;
            var fileType = CommonHelper.GetMimeTypeFromFileName(fileName);
            var fileToken = _guidGenerator.Create().ToString("N");
            byte[] fileBytes;

            using (var stream = formFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }
            _tempFileCacheManager.SetFile(fileToken, new TempFileInfo(fileName, fileType, fileBytes));
            result.Add(new FileDto() { FileToken = fileToken, FileName = formFile.FileName });
        }
        return Ok(result);
    }
    #endregion

}

