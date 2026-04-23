using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ADTOSharp.Auditing;
using ADTOSharp.Extensions;
using ADTOSharp.MimeTypes;
using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Controllers;
using ADTOSharp.Authorization;
using Microsoft.AspNetCore.Http;

namespace ADTO.DCloud.Web.Controllers
{
    /// <summary>
    /// 文件管理,用于管理下载的临时文件,这些文件一般存在内存中
    /// </summary>
    //[ADTOSharpAuthorize]
    [Route("api/[controller]/[action]")]
    public class FileController : DCloudControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IMimeTypeMap _mimeTypeMap;

        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager,
            IMimeTypeMap mimeTypeMap
        )
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _mimeTypeMap = mimeTypeMap;
        }

        /// <summary>
        /// 下载临时文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [DisableAuditing]
        [HttpPost]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);

            var fileInfo = _tempFileCacheManager.GetFileInfo(file.FileToken);
            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }
            Stream stream = new MemoryStream(fileBytes);
            IFormFile formFile = new FormFile(stream, 0, fileBytes.Length, "name", "fileName");

            return File(fileBytes, file.FileType, file.FileName);
        }
        /// <summary>
        /// 下载服务器文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [DisableAuditing]
        [HttpGet]
        public async Task<ActionResult> DownloadBinaryFile(Guid id, string contentType, string fileName)
        {
            var fileObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (fileObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            if (fileName.IsNullOrEmpty())
            {
                if (!fileObject.Description.IsNullOrEmpty() &&
                    !Path.GetExtension(fileObject.Description).IsNullOrEmpty())
                {
                    fileName = fileObject.Description;
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }
            }

            if (contentType.IsNullOrEmpty())
            {
                if (!Path.GetExtension(fileName).IsNullOrEmpty())
                {
                    contentType = _mimeTypeMap.GetMimeType(fileName);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }
            }

            return File(fileObject.Bytes, contentType, fileName);
        }
    }
}

