using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.FileManage;
using ADTO.DCloud.Media.FileManage.Utils;
using ADTO.DCloud.Media.UploadFiles.Dto;
using ADTO.DCloud.Migrations;
using ADTO.DCloud.Storage;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
//using Z.EntityFramework.Plus;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.Extensions.Configuration;

namespace ADTO.DCloud.Media.UploadFiles
{
    /// <summary>
    /// 文件上传保存操作
    /// </summary>
    public class UploadFileAppService : DCloudAppServiceBase, IUploadFileAppService
    {
        private readonly IRepository<UploadFile, Guid> _uploadFileRepository;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IFileManageService _fileManage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static IConfiguration _configuration;
        public UploadFileAppService(IRepository<UploadFile, Guid> uploadFileRepository
            , ITempFileCacheManager tempFileCacheManager
            , IGuidGenerator guidGenerator,
            IHttpContextAccessor httpContextAccessor,
             IConfiguration configuration,
             IFileManageService fileManage)
        {
            _httpContextAccessor = httpContextAccessor;
            _uploadFileRepository = uploadFileRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _guidGenerator = guidGenerator;
            _fileManage = fileManage; 
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
        #region Utils
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <returns></returns>
        private HttpContext GetRequestPath()
        {
            return IocManager.Instance.Resolve<IHttpContextAccessor>().HttpContext;
        }

        private HttpContext GetHttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// 添加图片表记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<UploadFile> InsertUploadFileAsync(CreateUploadFileDto input)
        {
            var dataItem = ObjectMapper.Map<UploadFile>(input);
            return await _uploadFileRepository.InsertAsync(dataItem);
        }
        [HiddenApi]
        public virtual string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }
        [HiddenApi]
        public virtual string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length <= maxLength)
                return str;

            var pLen = postfix?.Length ?? 0;

            var result = str[0..(maxLength - pLen)];
            if (!string.IsNullOrEmpty(postfix))
            {
                result += postfix;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 根据FolderId查询图片集合
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<List<UploadFileDto>> GetFileListByFolderId(Guid FolderId)
        {
            var query = this._uploadFileRepository.GetAll().Where(p => p.FolderId == FolderId);
            return ObjectMapper.Map<List<UploadFileDto>>(await query.ToListAsync());
        }

        /// <summary>
        /// 根据FolderIds查询附件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<List<UploadFilesInputDto>> GetFileList(GetFileListByFolderIdsInput input)
        {
            if (((input.FolderIds == null || input.FolderIds.Count <= 0) && (input.ProjectId == null || input.ProjectId == Guid.Empty)))
                return null;

            var query = await this._uploadFileRepository.GetAll()
                .WhereIf(input.FolderIds != null && input.FolderIds.Count() > 0, p => input.FolderIds.Contains(p.FolderId))
                .WhereIf(input.ProjectId != null && input.ProjectId != Guid.Empty, p => p.ProjectId.Equals(input.ProjectId))
                .ToListAsync();
            var list = query.Select(item =>
            {
                UploadFilesInputDto dto = new UploadFilesInputDto();
                dto.FileId = item.Id;
                var urlInfo = _fileManage.GetPublicFileUrl(item.VirtualPath);
                dto.Url = urlInfo.Result.ToString();
                dto.Name = item.FileName;
                dto.FolderId = item.FolderId;
                return dto;
            }).ToList();
            return list;
        }
        /// <summary>
        /// 根据FolderIds查询附件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<List<UploadFilesInputDto>> GetFileListByFolderIds(List<Guid> folderIds)
        {
            if (folderIds == null || folderIds.Count <= 0)
                return null;
            var query = await this._uploadFileRepository.GetAll().Where(p => folderIds.Contains(p.FolderId)).ToListAsync();
            var list = query.Select(item =>
            {
                UploadFilesInputDto dto = new UploadFilesInputDto();
                dto.FileId = item.Id;
                var urlInfo = _fileManage.GetPublicFileUrl(item.VirtualPath);
                dto.Url = urlInfo.Result.ToString();
                dto.Name = item.FileName;
                dto.FolderId = item.FolderId;
                return dto;
            }).ToList();
            return list;
        }

        /// <summary>
        /// 根据FolderIds查询附件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<List<UploadFilesInputDto>> GetFileListByProjectId(Guid projectId)
        {
            if (projectId == Guid.Empty)
                return null;
            var query = await this._uploadFileRepository.GetAll().Where(p => p.ProjectId.Equals(projectId)).ToListAsync();
            var list = query.Select(item =>
            {
                UploadFilesInputDto dto = new UploadFilesInputDto();
                dto.FileId = item.Id;
                var urlInfo = _fileManage.GetPublicFileUrl(item.VirtualPath);
                dto.Url = urlInfo.Result.ToString();
                dto.Name = item.FileName;
                dto.FolderId = item.FolderId;
                return dto;
            }).ToList();
            return list;
        }

        /// <summary>
        /// 根据FolderId查询图片集合,包含图片地址
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<List<UploadFileDto>> GetFileUrlListByFolderId(Guid FolderId)
        {
            var query = this._uploadFileRepository.GetAll().Where(p => p.FolderId == FolderId);
            var infoList = await query.ToListAsync();
            var list = infoList.Select(item =>
            {
                var dto = ObjectMapper.Map<UploadFileDto>(item);
                var fileInfo = GetPublicFileInfoByDto(dto);
                dto.FullAddress = fileInfo.FullAddress;
                return dto;
            }).ToList();

            return list;
        }

        #region MyRegion   
        /// <summary>
        /// 用户个人图像\登录界面图 保存地址
        /// </summary>
        public static string UploadHeadIconRootDirectory { get; } = @"Resource/PhotoFile";
        /// <summary>
        /// 上传文件至阿里云（图库上传）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<AjaxResponse<object>> UploadFilesUEditAsync()
        {
            AjaxResponse<object> result = new AjaxResponse<object> { Success = true };
            var httpContext = GetHttpContext();
            Guid folderId = Guid.NewGuid();
            if (httpContext.Request.Form.ContainsKey("folderId"))
            {
                folderId = Guid.Parse(httpContext.Request.Form["folderId"]);
            }
            //图片所属类型Id
            int filetypeId = 0;
            if (httpContext.Request.Form.ContainsKey("filetypeId"))
            {
                filetypeId = Convert.ToInt32(httpContext.Request.Form["filetypeId"]);
            }
            var imgExt = new List<string>
            {
                ".bmp",
                ".gif",
                ".webp",
                ".jpeg",
                ".jpg",
                ".jpe",
                ".jfif",
                ".pjpeg",
                ".pjp",
                ".png",
                ".tiff",
                ".tif"
            } as IReadOnlyCollection<string>;

            var uploadFiles = new List<UploadFileDto>();
            foreach (var formFile in httpContext.Request.Form.Files)
            {
                var fileName = string.IsNullOrWhiteSpace(formFile.FileName) ? formFile.Name : formFile.FileName;
                fileName = GetFileName(fileName);
                var mimeType = formFile.ContentType;
                var fileExtension = GetFileExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                if (!imgExt.All(ext => !ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (string.IsNullOrEmpty(mimeType))
                    {
                        switch (fileExtension)
                        {
                            case ".bmp":
                                mimeType = MimeTypes.ImageBmp;
                                break;
                            case ".gif":
                                mimeType = MimeTypes.ImageGif;
                                break;
                            case ".jpeg":
                            case ".jpg":
                            case ".jpe":
                            case ".jfif":
                            case ".pjpeg":
                            case ".pjp":
                                mimeType = MimeTypes.ImageJpeg;
                                break;
                            case ".webp":
                                mimeType = MimeTypes.ImageWebp;
                                break;
                            case ".png":
                                mimeType = MimeTypes.ImagePng;
                                break;
                            case ".tiff":
                            case ".tif":
                                mimeType = MimeTypes.ImageTiff;
                                break;
                            default:
                                break;
                        }
                    }
                    mimeType = mimeType ?? string.Empty;
                    mimeType = EnsureMaximumLength(mimeType, 20);
                }

                var fileId = Guid.NewGuid().ToString("D");
                //保存至阿里云
                //var resultModel = _aliyunService.PutFileAliyunNormal(formFile, directoryPath, fileId);

                //上传文件到阿里云
                var resultModel = await _fileManage.SaveFilesAsync(formFile);
                if (resultModel.Success)
                {
                    //插入文件表数据
                    CreateUploadFileDto createUploadFile = new CreateUploadFileDto()
                    {
                        FolderId = folderId,
                        VirtualPath = resultModel.Result.ToString(),
                        FileName = fileName,
                        MimeType = mimeType,
                        FileSize = formFile.Length.ToString(),
                        FileExtensions = fileExtension,
                        //FileTypeId = input.FileTypeId ?? Guid.Empty,
                        //ProjectId = input.ProjectId ?? Guid.Empty,
                        //EntityId = input.EntityId ?? Guid.Empty,
                    };
                    var upload = await InsertUploadFileAsync(createUploadFile);
                    uploadFiles.Add(ObjectMapper.Map<UploadFileDto>(upload));

                    var cdnDomainName = _configuration["Aliyun:cdnDomainName"];
                    //只添加了这里代表
                    await GetHttpContext().Response.WriteAsync(JsonConvert.SerializeObject(JObject.FromObject(
                    new
                    {
                        state = "SUCCESS",
                        //完整地址
                        url =$"{cdnDomainName}{resultModel.Result}" ,
                        title = fileName,
                        original = fileName,
                        error = string.Empty
                    })
                   ));

                }
            }
            result.Success = true;
            result.Result = uploadFiles;
            return result;
        }
        #endregion
        /// <summary>
        /// 从内存中取图片信息，保存文件（分两步上传图片，这是第二步  表单方式，后端方法内部调用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AjaxResponse<object>> UploadFileAsync(UploadSaveFileDto input)
        {
            AjaxResponse<object> response = new AjaxResponse<object> { Success = true };
            try
            {
                if (input.FileTokens.Count == 0)
                {
                    throw new UserFriendlyException("图片为空");
                }
                if (input.FolderId == Guid.Empty)
                {
                    input.FolderId = _guidGenerator.Create();
                }
                List<UploadFileDto> uploadFiles = new List<UploadFileDto>();
                var imgExt = new List<string>
                {
                    ".bmp",
                    ".gif",
                    ".webp",
                    ".jpeg",
                    ".jpg",
                    ".jpe",
                    ".jfif",
                    ".pjpeg",
                    ".pjp",
                    ".png",
                    ".tiff",
                    ".tif",
                    ".doc", ".docx", ".xls", ".xlsx", ".txt", ".pdf"
                } as IReadOnlyCollection<string>;
                foreach (var FileToken in input.FileTokens)
                {
                    //获取内存中的图片对象
                    var fileInfo = _tempFileCacheManager.GetFileInfo(FileToken);
                    if (fileInfo == null)
                    {
                        throw new UserFriendlyException("There is no such image file with the token: " + FileToken);
                    }

                    Stream stream = new MemoryStream(fileInfo.File);
                    //上传至阿里云或本地存储

                    var resultModel = await _fileManage.SaveFilesAsync(fileInfo);

                    if (resultModel.Success)
                    {
                        //插入文件表数据
                        CreateUploadFileDto createUploadFile = new CreateUploadFileDto()
                        {
                            FolderId = input.FolderId.Value,
                            VirtualPath = resultModel.Result.ToString(),
                            FileName = fileInfo.FileName,
                            MimeType = fileInfo.FileType,
                            FileSize = fileInfo.File.Length.ToString(),
                            FileExtensions = Path.GetExtension(fileInfo.FileName),

                            FileTypeId = input.FileTypeId ?? Guid.Empty,
                            ProjectId = input.ProjectId ?? Guid.Empty,
                            EntityId = input.EntityId ?? Guid.Empty,
                        };
                        var upload = await InsertUploadFileAsync(createUploadFile);
                        uploadFiles.Add(ObjectMapper.Map<UploadFileDto>(upload));
                    }
                }
                response.Result = uploadFiles;
            }
            catch (Exception ex)
            {
                response.Success = false;
                throw new UserFriendlyException(ex.Message);
            }
            return response;
        }

        /// <summary>
        /// 选择文件直接上传且保存文件,不暂存在内存，直接保存（非表单方式，前端调用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [IgnoreAntiforgeryToken]
        public async Task<AjaxResponse<object>> UploadSaveFileAsync([FromForm] UploadSaveFileDto input)
        {
            AjaxResponse<object> response = new AjaxResponse<object> { Success = true };
            try
            {
                if (input.FolderId == null || input.FolderId == Guid.Empty)
                {
                    input.FolderId = _guidGenerator.Create();
                }

                List<UploadFileDto> uploadFiles = new List<UploadFileDto>();
                var imgExt = new List<string>
                {
                    ".bmp",
                    ".gif",
                    ".webp",
                    ".jpeg",
                    ".jpg",
                    ".jpe",
                    ".jfif",
                    ".pjpeg",
                    ".pjp",
                    ".png",
                    ".tiff",
                    ".tif",
                    ".doc", ".docx", ".xls", ".xlsx", ".txt", ".pdf"
                } as IReadOnlyCollection<string>;
                foreach (var formFile in GetRequestPath().Request.Form.Files)
                {
                    var fileName = string.IsNullOrWhiteSpace(formFile.FileName) ? formFile.Name : formFile.FileName;
                    fileName = GetFileName(fileName);

                    var mimeType = formFile.ContentType;
                    var fileExtension = GetFileExtension(fileName);
                    if (!string.IsNullOrEmpty(fileExtension))
                        fileExtension = fileExtension.ToLowerInvariant();

                    if (imgExt.All(ext => !ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        throw new UserFriendlyException("文件格式错误");
                    }
                    //上传文件到阿里云
                    var resultModel = await _fileManage.SaveFilesAsync(formFile);

                    if (resultModel.Success)
                    {
                        //插入文件表数据
                        CreateUploadFileDto createUploadFile = new CreateUploadFileDto()
                        {
                            FolderId = input.FolderId.Value,
                            VirtualPath = resultModel.Result.ToString(),
                            FileName = formFile.FileName,
                            MimeType = mimeType,
                            FileSize = formFile.Length.ToString(),
                            FileExtensions = Path.GetExtension(formFile.FileName),

                            FileTypeId = input.FileTypeId ?? Guid.Empty,
                            ProjectId = input.ProjectId ?? Guid.Empty,
                            EntityId = input.EntityId ?? Guid.Empty,
                        };
                        var upload = await InsertUploadFileAsync(createUploadFile);
                        var uploadfileDto = ObjectMapper.Map<UploadFileDto>(upload);
                        var fileInfo = GetPublicFileInfoByDto(uploadfileDto);
                        uploadfileDto.FullAddress = fileInfo.FullAddress;
                        uploadFiles.Add(uploadfileDto);
                    }
                }
                response.Result = uploadFiles;
            }
            catch (Exception ex)
            {
                response.Success = false;
                throw new UserFriendlyException(ex.Message);
            }
            return response;
        }

        #region
        /// <summary>
        /// 获取公共文件、图片完整地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public UploadFileDto GetPublicFileInfoByDto(UploadFileDto input)
        {
            // 获取文件URL
            var urlInfo = _fileManage.GetPublicFileUrl(input.VirtualPath);

            var result = new UploadFileDto
            {
                Id = input.Id,
                FileName = input.FileName,
                VirtualPath = input.VirtualPath,
                FullAddress = urlInfo?.Result.ToString() ?? string.Empty
            };
            return result;
        }
        /// <summary>
        /// 获取公共文件、图片完整地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<UploadFileDto> GetPublicFileInfo(EntityDto<Guid> input)
        {
            var entity = await this._uploadFileRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var urlInfo = _fileManage.GetPublicFileUrl(entity.VirtualPath);

            var uploadDto = ObjectMapper.Map<UploadFileDto>(entity);
            uploadDto.FullAddress = urlInfo.Result.ToString();
            return uploadDto;
        }

        /// <summary>
        /// 获取私有文件、图片完整地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<UploadFileDto> GetPrivateFileInfo(EntityDto<Guid> input)
        {
            var entity = await this._uploadFileRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var urlInfo = _fileManage.GetPrivateFileUrl(entity.VirtualPath);

            var uploadDto = ObjectMapper.Map<UploadFileDto>(entity);
            uploadDto.FullAddress = urlInfo.Result.ToString();
            return uploadDto;
        }

        #endregion

        /// <summary>
        /// 查询图片分类列表（图片库组件）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UploadFileDto>> GetUploadFilePageListAsync(PagedFileResultRequestDto input)
        {
            var query = this._uploadFileRepository.GetAll()
                .WhereIf(input.ProjectId != null && input.ProjectId != Guid.Empty, p => p.ProjectId == input.ProjectId)
                .WhereIf(input.FileTypeId != null && input.FileTypeId != Guid.Empty, p => p.FileTypeId == input.FileTypeId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.FileName), p => p.FileName.Contains(input.FileName));

            var totalCount = await query.CountAsync();
            //根据更新日期排序
            var items = await query.OrderByDescending(p => p.CreationTime).PageBy(input).ToListAsync();

            var list = new List<UploadFileDto>();
            foreach (var item in items)
            {
                var dto = ObjectMapper.Map<UploadFileDto>(item);
                var fileInfo = GetPublicFileInfoByDto(dto);
                dto.FullAddress = fileInfo.FullAddress;
                list.Add(dto);
            }
            return new PagedResultDto<UploadFileDto>(totalCount, list);
        }

        /// <summary>
        /// 批量修改图片所属类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateFileTypeIdAsync(UpdateFileTypeIdDto input)
        {
            if (input.FileIds.Count <= 0)
            {
                throw new UserFriendlyException("操作失败：请选择图片");
            }
             

            foreach (var item in input.FileIds)
            {
                await this._uploadFileRepository.UpdateAsync(item, async entity =>
                {
                    entity.FileTypeId = input.FileTypeId;
                });
            }

            //_uploadFileRepository.BatchUpdateAsync()
            //批量修改满足条件的数据
            //(await _uploadFileRepository.GetAll().Where(p => input.FileIds.Contains(p.Id)).ForEach(s => {


            //});


            //    .UpdateAsync(p => new UploadFile
            //{
            //    FileTypeId = input.FileTypeId,
            //});
        }
        /// <summary>
        /// 上传附件-20260115新增
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="list"></param>
        /// <param name="projectId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<string> UploadFolderFileAsync(string folderId, List<UploadFilesInputDto> list, Guid? projectId = null, Guid? entityId = null)
        {
            if (string.IsNullOrWhiteSpace(folderId) && (list == null || list.Count <= 0))
                return "";
            if (string.IsNullOrWhiteSpace(folderId) || folderId == Guid.Empty.ToString())
            {
                folderId = _guidGenerator.Create().ToString();
            }
            else if (folderId != Guid.Empty.ToString())
            {
                //校验是否删除了之前的图片
                //原本所有的图片
                var existingFiles = await this.GetFileListByFolderId(Guid.Parse(folderId));
                // 找出被删除的文件
                var deletedFileIds = existingFiles
                    .ExceptBy(list.Select(f => f.FileId), f => f.Id)
                    .Select(f => f.Id)
                    .ToList();
                //批量删除
                if (deletedFileIds.Count > 0)
                {
                    await this.DeleteUploadFileListAsync(deletedFileIds);
                }
                //await this.DeleteUploadFileByFolderId2Async(folderId, list);
            }
            if (list == null || list.Count < 1)
                return "";

            List<string> FileTokensList = list.Where(q => !string.IsNullOrWhiteSpace(q.FileToken)).Select(d => d.FileToken).ToList();
            if (FileTokensList.Count > 0)
            {
                await this.UploadFileAsync(new Media.UploadFiles.Dto.UploadSaveFileDto()
                {
                    FolderId = Guid.Parse(folderId),
                    FileTokens = FileTokensList,
                    ProjectId = projectId,
                    EntityId = entityId,
                });
            }
            return folderId;
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteUploadFileByFolderId2Async(Guid folderId, List<UploadFilesInputDto> list)
        {
            //校验是否删除了之前的图片
            //原本所有的图片
            var existingFiles = await this.GetFileListByFolderId(folderId);
            // 找出被删除的文件
            var deletedFileIds = existingFiles
                .ExceptBy(list.Select(f => f.FileId), f => f.Id)
                .Select(f => f.Id)
                .ToList();
            //批量删除
            if (deletedFileIds.Count > 0)
            {
                await this.DeleteUploadFileListAsync(deletedFileIds);
            }
        }

        /// <summary>
        /// 删除图片（逻辑删除）
        /// </summary>
        /// <param name="input">图片Id</param>
        /// <returns></returns>
        public async Task DeleteUploadFileAsync(EntityDto<Guid> input)
        {
            var vInfo = await _uploadFileRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (vInfo == null)
            {
                throw new UserFriendlyException("操作失败：不存在此记录");
            }
            await _uploadFileRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 批量删除，根据图片Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteUploadFileListAsync(List<Guid> input)
        {
            await _uploadFileRepository.BatchDeleteAsync(p => input.Contains(p.Id));
        }

        /// <summary>
        /// 批量删除，根据FolderId
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteUploadFileByFolderIdAsync(Guid FolderId)
        {
            await _uploadFileRepository.BatchDeleteAsync(p => p.FolderId == FolderId);
        }

        public virtual string GetUEditorConfiguration()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\lib\\ueditor\\ueditor.config.json");
            return ReadAllText(filePath, Encoding.UTF8);
        }
        /// <summary>
        /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading</param>
        /// <param name="encoding">The encoding applied to the contents of the file</param>
        /// <returns>A string containing all lines of the file</returns>
        public virtual string ReadAllText(string path, Encoding encoding)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream, encoding);

            return streamReader.ReadToEnd();
        }
        /// <summary>
        /// Get the string to write an error response
        /// </summary>
        /// <param name="message">Additional message</param>
        /// <returns>String to write to the response</returns>
        public virtual string GetErrorResponse(string message = null)
        {
            return GetResponse("error", message);
        }
        /// <summary>
        /// Get the string to write to the response
        /// </summary>
        /// <param name="type">Type of the response</param>
        /// <param name="message">Additional message</param>
        /// <returns>String to write to the response</returns>
        protected virtual string GetResponse(string type, string message)
        {
            return $"{{\"res\":\"{type}\",\"msg\":\"{message?.Replace("\"", "\\\"")}\"}}";
        }

        /// <summary>
        /// Whether the request is made with ajax 
        /// </summary>
        /// <returns>True or false</returns>
        public virtual bool IsAjaxRequest()
        {
            return GetHttpContext().Request.Form != null &&
                   !StringValues.IsNullOrEmpty(GetHttpContext().Request.Form["method"]) &&
                   GetHttpContext().Request.Form["method"] == "ajax";
        }
    }
}

