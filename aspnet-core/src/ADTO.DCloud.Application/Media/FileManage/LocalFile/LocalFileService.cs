using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.FileManage.Aliyun;
using ADTO.DCloud.Media.UploadFiles.Dto;
using ADTO.DCloud.Storage;
using ADTOSharp.Web.Models;
using Aliyun.OSS.Common;
using Aliyun.OSS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ADTO.DCloud.Media.FileManage.Utils;

namespace ADTO.DCloud.Media.FileManage.LocalFile
{
    /// <summary>
    /// 本地存储文件
    /// </summary>
    public class LocalFileService : DCloudDomainServiceBase, ILocalFileService, IFileManageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDCloudFileProvider _fileProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public LocalFileService(IDCloudFileProvider fileProvider, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _fileProvider = fileProvider;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetFullPath(string virtualPath)
        {
            virtualPath ??= string.Empty;
            if (!virtualPath.StartsWith("/"))
                virtualPath = "/" + virtualPath;
            virtualPath = virtualPath.TrimEnd('/');
            return _fileProvider.Combine(_webHostEnvironment.WebRootPath, virtualPath);
        }

        /// <summary>
        /// 获取当前域名
        /// </summary>
        /// <returns></returns>
        public string GetApiDomain()
        {
            HttpRequest request = _httpContextAccessor.HttpContext.Request;
            string domain = $"{request.Scheme}://{request.Host}";
            return domain;
        }

        /// <summary>
        /// 文件上传（内存方式）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<AjaxResponse<object>> SaveFilesAsync(TempFileInfo fileInfo)
        {
            var result = new AjaxResponse<object>();
            string directoryPath = MediaDefaults.DefaultUploadedRootDirectory;
            string virtualPath = Path.Combine(directoryPath, DateTime.Today.ToString("yyyyMMdd"));
            string localPath = Path.Combine(_webHostEnvironment.WebRootPath, virtualPath);
            directoryPath = new DirectoryInfo(GetFullPath(localPath)).FullName;
            if (!_fileProvider.DirectoryExists(directoryPath))
            {
                _fileProvider.CreateDirectory(directoryPath);
            }
            try
            {
                var uploadFiles = new List<UploadFileDto>();

                var fileName = fileInfo.FileName;
                //fileName = _fileProvider.GetFileName(fileName);
                //图片类型
                var mimeType = fileInfo.FileType;
                //后缀
                var fileExtension = _fileProvider.GetFileExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();
                byte[] fileBinary = fileInfo.File;

                //var fileId = Guid.NewGuid().ToString("D");
                //var destinationFileName = $"{fileId}{fileExtension}";

                //var destinationFilePath = _fileProvider.Combine(directoryPath, destinationFileName);
                var destinationFilePath = _fileProvider.Combine(directoryPath, fileName);
                await _fileProvider.WriteAllBytesAsync(destinationFilePath, fileBinary);
                result.Result = Path.Combine(virtualPath, fileName);
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 文件上传(非内存方式，直接调用保存)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<AjaxResponse<object>> SaveFilesAsync(IFormFile formFile)
        {
            var result = new AjaxResponse<object>();
            string directoryPath = MediaDefaults.DefaultUploadedRootDirectory;
            string virtualPath = Path.Combine(directoryPath, DateTime.Today.ToString("yyyyMMdd"));
            string localPath = Path.Combine(_webHostEnvironment.WebRootPath, virtualPath);
            directoryPath = new DirectoryInfo(GetFullPath(localPath)).FullName;
            if (!_fileProvider.DirectoryExists(directoryPath))
            {
                _fileProvider.CreateDirectory(directoryPath);
            }
            try
            {
                var uploadFiles = new List<UploadFileDto>();
                var fileName = formFile.FileName;
                //后缀
                var fileExtension = _fileProvider.GetFileExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();
                byte[] fileBinary = await GetFileBitsAsync(formFile);


                var destinationFilePath = _fileProvider.Combine(directoryPath, fileName);
                await _fileProvider.WriteAllBytesAsync(destinationFilePath, fileBinary);
                result.Result = Path.Combine(virtualPath, fileName);
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error.Message = ex.Message;
            }
            return result;
        }
        private async Task<byte[]> GetFileBitsAsync(IFormFile file)
        {
            await using var fileStream = file.OpenReadStream();
            await using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            return fileBytes;
        }

        /// <summary>
        /// 获取公共文件访问地址
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public AjaxResponse<object> GetPublicFileUrl(string objectName)
        {
            AjaxResponse<object> response = new AjaxResponse<object>();
            var fullUrl = Path.Combine(GetApiDomain(), objectName);
            response.Result = fullUrl.Replace(@"\", "/");
            return response;
        }

        /// <summary>
        ///  获取私有文件访问地址(本地文件，不存在私有文件，是针对OSS)
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public AjaxResponse<object> GetPrivateFileUrl(string objectName)
        {
            AjaxResponse<object> response = new AjaxResponse<object>();
            var fullUrl = Path.Combine(GetApiDomain(), objectName);
            response.Result = fullUrl.Replace(@"\", "/");
            return response;
        }
    }
}

