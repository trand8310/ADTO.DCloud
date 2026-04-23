using ADTO.DCloud;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.FileManage.Aliyun;
using ADTO.DCloud.Media.FileManage.Aliyun.Dto;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Storage.Dto;
using ADTO.DCloud.Url;
using ADTO.OA.Storage.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.IO.Extensions;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;


namespace ADTO.DCloud.Storage
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public class FileExplorerAppService : DCloudAppServiceBase, IFileExplorerAppService
    {
        #region Fields
        private readonly IRepository<SharedFileInfo, Guid> _sharedFileInfoRepository;
        private readonly IRepository<SharedFileCategory, Guid> _sharedFileCategoryRepository;
        private readonly IRepository<SharedFileAuthorizes, Guid> _sharedFileAuthorizeRepository;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IDownloadService _downloadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDCloudFileProvider _fileProvider;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IAliyunFileService _aliyunService;
        private static IConfigurationRoot _appConfiguration;
        private readonly UserManager _userManager;
        private readonly IDataFilterService _dataFilterService;
        private readonly IGuidGenerator _guidGenerator;

        private readonly IWebHostEnvironment _env;
        //private readonly IConfigurationRoot _appConfiguration;
        #endregion

        #region Ctor

        public FileExplorerAppService(
            IWebHostEnvironment env,
            IRepository<SharedFileInfo, Guid> sharedFileInfoRepository,
            IRepository<SharedFileCategory, Guid> sharedFileCategoryRepository,
            IRepository<SharedFileAuthorizes, Guid> sharedFileAuthorizeRepository,
            ITempFileCacheManager tempFileCacheManager,
            IDownloadService downloadService,
            IUrlRecordService urlRecordService,
            IHttpContextAccessor httpContextAccessor,
            IDCloudFileProvider fileProvider,
            IAliyunFileService aliyunService,
            UserManager userManager,
            IDataFilterService dataFilterService,
            IGuidGenerator guidGenerator)
        {
            _sharedFileInfoRepository = sharedFileInfoRepository;
            _sharedFileCategoryRepository = sharedFileCategoryRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _downloadService = downloadService;
            _urlRecordService = urlRecordService;
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
            _aliyunService = aliyunService;
            _sharedFileAuthorizeRepository = sharedFileAuthorizeRepository;
            _userManager = userManager;
            _dataFilterService = dataFilterService;
            _guidGenerator = guidGenerator;
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        #endregion

        #region Utilities
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HttpContext GetHttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        private List<TreeViewCreateModel<Guid>> GenerateCategoryTree(List<FileCategoryDto> list, Guid? parentId = null)
        {
            var query = list.AsQueryable().
                WhereIf(parentId.HasValue, w => w.Parent != null && w.Parent.Id == parentId.Value)
                .WhereIf(!parentId.HasValue, w => w.Parent == null).OrderBy(d => d.SortCode).ToList();

            return query.Select(item =>
            {
                var m = new TreeViewCreateModel<Guid>
                {
                    Id = item.Id,
                    Label = item.Name,
                    Children = GenerateCategoryTree(list, item.Id),
                    CreateUserId = item.CreatorUserId,
                };
                if (parentId.HasValue)
                {
                    m.ParentId = parentId.Value;
                }
                m.IsLeaf = m.Children.Count() == 0;
                return m;
            }).ToList();
        }



        #endregion

        /// <summary>
        /// 上传文件到缓冲区,并返回文件的TOKENID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<List<FileTokenDto>> UploadFileAsync()
        {
            var httpContext = this.GetHttpContext();
            var result = new List<FileTokenDto>();
            foreach (var formFile in httpContext.Request.Form.Files)
            {
                var fileName = formFile.FileName;
                var fileType = CommonHelper.GetMimeTypeFromFileName(fileName);
                var fileToken = SequentialGuidGenerator.Instance.Create().ToString("D");
                byte[] fileBytes;
                using (var stream = formFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                //直接上传阿里云
                var response = _aliyunService.SaveFile(formFile);
                //var fileInfo = JsonConvert.DeserializeObject<AliyunPutObjectDto>(response.Data);

                await _tempFileCacheManager.SetFileAsync(fileToken,
                    new TempFileInfo()
                    {
                        Length = response.Data.ContentLength,
                        FileName = fileName,
                        OriginalName = formFile.FileName,
                        FileType = fileType
                    });

                result.Add(new FileTokenDto() { FileToken = fileToken, FileName = formFile.FileName, FileSize = fileBytes.Length, MimeType = fileType });

                #region 保存缓存再上传
                //var fileName = formFile.FileName;
                //var fileType = CommonHelper.GetMimeTypeFromFileName(fileName);
                //var fileToken = SequentialGuidGenerator.Instance.Create().ToString("D");
                //byte[] fileBytes;
                //using (var stream = formFile.OpenReadStream())
                //{
                //    fileBytes = stream.GetAllBytes();
                //}
                //await _tempFileCacheManager.SetFileAsync(fileToken, new TempFileInfo(fileName, fileType, fileBytes));
                //result.Add(new FileTokenDto() { FileToken = fileToken, FileName = formFile.FileName, FileSize = fileBytes.Length, MimeType = fileType });
                #endregion
            }
            return result;
        }
        public async void SetLargeFileAsync(string fileToken, TempFileInfo fileInfo)
        {
            const int chunkSize = 512 * 1024; // 512KB 每块
            var fileBytes = fileInfo.File;
            for (int i = 0; i < fileBytes.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, fileBytes.Length - i);
                byte[] chunk = new byte[length];
                System.Array.Copy(fileBytes, i, chunk, 0, length);

                string chunkKey = $"{fileToken}:chunk:{i / chunkSize}";
                await _tempFileCacheManager.SetFileAsync(chunkKey,
                    new TempFileInfo(fileInfo.FileName, fileInfo.FileType, chunk));
            }
            // 存储元数据
            var metadata = new
            {
                FileName = fileInfo.FileName,
                FileType = fileInfo.FileType,
                TotalSize = fileBytes.Length,
                ChunkCount = (int)Math.Ceiling(fileBytes.Length / (double)chunkSize)
            };
            await _tempFileCacheManager.SetFileAsync($"{fileToken}:metadata",
                new TempFileInfo("metadata.json", "application/json",
                    Encoding.UTF8.GetBytes(metadata.ToJson())));
        }

        #region 文件目录

        /// <summary>
        /// 按页获取文件目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("文件管理中心")]
        public async Task<PagedResultDto<FileCategoryDto>> GetFileCategories(PagedFileCategoryResultRequestDto input)
        {

            //var currentUser = await _userManager.GetUserByIdAsync(ADTOSharpSession.GetUserId());
            var query = (from ct in _sharedFileCategoryRepository.GetAll()
                         join auth in _sharedFileAuthorizeRepository.GetAll()
                         on ct.Id equals auth.SharedFileCategory into bGroup
                         from auth in bGroup.DefaultIfEmpty()
                         select new GetFileCategoriesDto { SharedFileCategory = ct, SharedFileAuthorize = auth });

            //数据权限
            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetFileCategories)));
            query = await _dataFilterService.CreateDataFilteredQuery(query, permissionCode);


            //var filteredQuery = await _permissionAppServiceService
            //    .CreateDataFilteredQueryWithFields<GetFileCategoriesDto>(query, this.GetRequestPath().Replace("ExportExcel", ""))
            //    ;

            var list = query.DistinctBy(dto => dto.SharedFileCategory.Id);
            var resultCount = await list.CountAsync();

            var results = await list
                .PageBy(input)
                .OrderByDescending(o => o.SharedFileCategory.CreationTime)
                .ToListAsync();
            return new PagedResultDto<FileCategoryDto>(resultCount, ObjectMapper.Map<List<FileCategoryDto>>(results));
        }

        /// <summary>
        /// 生成文件目录结构
        /// </summary>
        /// <returns></returns>
        public async Task<List<TreeViewCreateModel<Guid>>> GetFileCategoriesTree()
        {
            //当前登录用户
            var currentUser = await _userManager.GetUserByIdAsync(ADTOSharpSession.GetUserId());

            var query = (from ct in _sharedFileCategoryRepository.GetAll()
                         join auth in _sharedFileAuthorizeRepository.GetAll()
                         on ct.Id equals auth.SharedFileCategory into bGroup
                         from auth in bGroup.DefaultIfEmpty()
                         where auth == null ||
                         (
                             currentUser.Id == AppConsts.AdminUserId1 ||
                             currentUser.Id == AppConsts.AdminUserId2 ||
                             auth.ObjectId == currentUser.DepartmentId
                         )
                         select ct)
                         .Distinct();

            var categories = (query.ToList().Where(q => q.IsDeleted == false).OrderBy(d => d.SortCode).Select(item =>
            {
                var model = ObjectMapper.Map<FileCategoryDto>(item);
                return model;
            }).ToList());
            var data = this.GenerateCategoryTree(categories, null);
            return data;
        }

        /// <summary>
        /// 依ID获取一个文件目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileCategoryDto> GetFileCategoryAsync(EntityDto<Guid> input)
        {
            var model = await _sharedFileCategoryRepository.GetAsync(input.Id);
            var authorizes = await _sharedFileAuthorizeRepository.GetAll().Where(q => q.ObjectType == "Department" && q.SharedFileCategory == input.Id).ToListAsync();
            var _model = ObjectMapper.Map<FileCategoryDto>(model);
            _model.Permissions = authorizes.Select(item =>
               {
                   var cDto = ObjectMapper.Map<SharedFileAuthorizeDto>(item);
                   return cDto;
               }).ToList();
            return _model;
        }


        public async Task<FileCategoryDto> CreateFileCategoryAsync(CreateFileCategoryDto input)
        {
            var entity = ObjectMapper.Map<SharedFileCategory>(input);
            entity.ParentCategoryId = entity.ParentCategoryId == Guid.Empty ? null : entity.ParentCategoryId;
            var id = await _sharedFileCategoryRepository.InsertAndGetIdAsync(entity);
            if (input.Permissions != null)
            {
                foreach (var item in input.Permissions)
                {
                    item.SharedFileCategory = id;
                    var entityAuth = ObjectMapper.Map<SharedFileAuthorizes>(item);
                    await _sharedFileAuthorizeRepository.InsertAsync(entityAuth);
                }
            }
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<FileCategoryDto>(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<JsonResultModel> UploadFilesAndFoldersAsync(UploadFileFolderDto input)
        {
            var result = new JsonResultModel();
            try
            {
                var folderList = await _sharedFileCategoryRepository.GetAllListAsync();
                List<SharedFileInfo> sharedFileInfos = new List<SharedFileInfo>();
                await SaveFilesAndFolders(input.CategoryId, input.FolderDtos, folderList, sharedFileInfos);
                foreach (var folder in sharedFileInfos)
                {
                    await _sharedFileInfoRepository.InsertAsync(folder);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex.ToString();
            }
            return result;
        }


        public async Task<bool> SaveFilesAndFolders(Guid? parentId, List<FolderDto> list, List<SharedFileCategory> folderList, List<SharedFileInfo> sharedFileInfos)
        {
            foreach (var dto in list)
            {
                switch (dto.Type)
                {
                    case "folder"://文件夹
                        var folder = folderList.FirstOrDefault(d => (parentId.HasValue ? d.ParentCategoryId == parentId : d.ParentCategoryId == null) && d.Name == dto.Name);
                        if (folder == null || folder.Id == Guid.Empty)
                        {
                            var categoryId = await _sharedFileCategoryRepository.InsertAndGetIdAsync(new SharedFileCategory()
                            {
                                Name = dto.Name,
                                ParentCategoryId = parentId.HasValue && parentId != Guid.Empty ? parentId : null
                            });
                            await this.SaveFilesAndFolders(categoryId, dto.Children, folderList, sharedFileInfos);
                        }
                        else
                        {
                            Guid? categoryId = folder != null && folder.Id != Guid.Empty ? folder.Id : parentId;
                            await this.SaveFilesAndFolders(categoryId, dto.Children, folderList, sharedFileInfos);
                        }
                        break;
                    default:
                        var fileInfo = new SharedFileInfo();
                        fileInfo.CategoryId = parentId == Guid.Empty ? null : parentId;
                        fileInfo.OriginalName = dto.OriginalName;
                        fileInfo.FileName = dto.FileName;
                        fileInfo.FileSize = dto.FileSize;
                        fileInfo.MimeType = dto.MimeType;
                        fileInfo.FileExtension = dto.FileExtension;
                        fileInfo.VirtualPath = dto.VirtualPath;
                        //await _sharedFileInfoRepository.InsertAsync(fileInfo);
                        sharedFileInfos.Add(fileInfo);
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// 编辑文件目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileCategoryDto> UpdateFileCategoryAsync(UpdateFileCategoryDto input)
        {
            var entity = await _sharedFileCategoryRepository.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("文件目录不存在");
            }
            ObjectMapper.Map(input, entity);
            var model = await _sharedFileCategoryRepository.UpdateAsync(entity);
            var authorizesObjIds = await _sharedFileAuthorizeRepository.GetAll().Where(w => w.SharedFileCategory.Equals(model.Id)).Select(d => d.ObjectId).ToListAsync();
            //删除不再需要的信息
            await _sharedFileAuthorizeRepository.DeleteAsync(d => d.SharedFileCategory.Equals(model.Id) && !input.Permissions.Select(d => d.ObjectId).Any(a => a.Equals(d.ObjectId)));
            var authorizeList = input.Permissions.Where(q => !authorizesObjIds.Any(a => a.Equals(q.ObjectId)));
            foreach (var item in authorizeList)
            {
                item.SharedFileCategory = entity.Id;
                var entityAuth = ObjectMapper.Map<SharedFileAuthorizes>(item);
                await _sharedFileAuthorizeRepository.InsertAsync(entityAuth);
            }
            return ObjectMapper.Map<FileCategoryDto>(model);
        }

        /// <summary>
        /// 删除文件目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<FileCategoryDto> DeleteFileCategoryAsync(EntityDto<Guid> input)
        {
            var entity = await _sharedFileCategoryRepository.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("文件目录不存在");
            }
            if (await _sharedFileCategoryRepository.GetAllIncluding(p => p.Parent).AnyAsync(w => w.Parent != null && w.Parent.Id == input.Id))
            {
                throw new UserFriendlyException("该目录下面还存在子目录,不能删除");
            }
            if (await _sharedFileInfoRepository.GetAll().AnyAsync(w => w.CategoryId == input.Id))
            {
                throw new UserFriendlyException("该目录下面还存在文件,不能删除");
            }
            await _sharedFileCategoryRepository.DeleteAsync(entity);
            return ObjectMapper.Map<FileCategoryDto>(entity);
        }

        #endregion

        #region 文件

        /// <summary>
        /// 按页获取文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<FileInfoLiteDto>> GetFileInfos(PagedFileInfoResultRequestDto input)
        {
            var query = _sharedFileInfoRepository.GetAllIncluding(p => p.Category)
                .WhereIf(input.CategoryId.HasValue, w => w.CategoryId == (input.CategoryId == Guid.Empty ? null : input.CategoryId))
                .WhereIf(!input.Keyword.IsNullOrEmpty(), w => w.OriginalName.Contains(input.Keyword));

            var totalCount = query.Count();
            var resultCount = await query.CountAsync();
            var results = await query
                .OrderByDescending(o => o.CreationTime)
                .PageBy(input)
                .ToListAsync();

            var cdnDomainName = _appConfiguration["Aliyun:cdnDomainName"];
            var list = results.Select(item =>
            {
                var model = ObjectMapper.Map<FileInfoLiteDto>(item);
                model.Category = ObjectMapper.Map<FileCategoryLiteDto>(item.Category);
                //model.Url = $"{item.VirtualPath}{item.FileName}";
                return model;
            }).ToList();
            return new PagedResultDto<FileInfoLiteDto>(resultCount, list);
        }


        /// <summary>
        /// 依ID获取一个文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileInfoLiteDto> GetFileInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _sharedFileInfoRepository.GetAllIncluding(x => x.Category).FirstOrDefaultAsync(w => w.Id == input.Id);
            var model = ObjectMapper.Map<FileInfoLiteDto>(entity);
            model.Category = ObjectMapper.Map<FileCategoryLiteDto>(entity.Category);
            //model.Url = $"Storage/{entity.VirtualPath}/{entity.FileName}";
            return model;
        }


        /// <summary>
        /// 新增文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        //[AbpAuthorize("Pages.FileManager.FileList.Add")]
        public async Task<FileInfoDto> CreateSharedFileInfoAsync(SharedFileInfoDto input)
        {
            if (string.IsNullOrWhiteSpace(input.FileName) || string.IsNullOrEmpty(input.OriginalName))
            {
                throw new UserFriendlyException("文件名不能为空");
            }
            var fileInfo = new SharedFileInfo();
            fileInfo.CategoryId = input.CategoryId == Guid.Empty ? null : input.CategoryId;
            fileInfo.Description = input.Description;
            fileInfo.OriginalName = input.OriginalName;
            fileInfo.FileName = input.FileName;
            fileInfo.FileSize = input.FileSize;
            fileInfo.MimeType = input.MimeType;
            fileInfo.FileExtension = input.FileExtension;
            fileInfo.VirtualPath = input.VirtualPath;
            await _sharedFileInfoRepository.InsertAsync(fileInfo);
            return ObjectMapper.Map<FileInfoDto>(fileInfo);
        }

        /// <summary>
        /// 新增文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
            //[AbpAuthorize("Pages.FileManager.FileList.Add")]
        public async Task<FileInfoDto> CreateFileInfoAsync(CreateFileInfoDto input)
        {
            if (input.FileToken.IsNullOrEmpty())
            {
                throw new UserFriendlyException("文件标识不能为空");
            }
            if (input.CategoryId == Guid.Empty)
            {
                throw new UserFriendlyException("文件目录不能为空");
            }
            var category = await _sharedFileCategoryRepository.FirstOrDefaultAsync(p => p.Id == input.CategoryId);
            if (category == null)
            {
                throw new UserFriendlyException("文件目录无效");
            }
            var tempFileInfo = _tempFileCacheManager.GetFileInfo(input.FileToken);
            if (tempFileInfo == null)
            {
                throw new UserFriendlyException("文件标识无效");
            }
            //判断上传的附件是否大于指定的附件的大小，如果是则保存在本地服务器，不是保存阿里云服务器
            var fileInfo = new SharedFileInfo();

            if (fileInfo == null)
            {
                throw new UserFriendlyException("文件上传发生异常");
            }
            fileInfo.CategoryId = input.CategoryId;
            fileInfo.Description = input.Description;
            fileInfo.OriginalName = tempFileInfo.OriginalName;
            fileInfo.FileName = tempFileInfo.FileName;
            fileInfo.FileSize = tempFileInfo.Length;
            fileInfo.MimeType = tempFileInfo.FileType;
            await _sharedFileInfoRepository.InsertAsync(fileInfo);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<FileInfoDto>(fileInfo);
        }


        /// <summary>
        /// 编辑文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileInfoDto> UpdateFileInfoAsync(UpdateFileInfoDto input)
        {

            if (!input.CategoryId.HasValue || input.CategoryId == Guid.Empty)
            {
                throw new UserFriendlyException("文件目录不能为空1");
            }

            var category = await _sharedFileCategoryRepository.FirstOrDefaultAsync(p => p.Id == input.CategoryId);
            if (category == null)
            {
                throw new UserFriendlyException("文件目录无效");
            }
            var entity = await _sharedFileInfoRepository.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("文件不存在");
            }
            var model = _sharedFileInfoRepository.Update(input.Id, e =>
             {
                 e.Description = input.Description;
                 e.CategoryId = category.Id;
             });
            return ObjectMapper.Map<FileInfoDto>(model);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<FileInfoDto> DeleteFileInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _sharedFileInfoRepository.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("文件不存在");
            }
            await _sharedFileInfoRepository.DeleteAsync(entity);
            return ObjectMapper.Map<FileInfoDto>(entity);
        }
        #endregion

        #region 获取token

        /// <summary>
        /// 获取OSS临时身份凭证
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [ADTOSharpAuthorize]
        public JsonResultModel GetOssTSTAccessKey()
        {
            try
            {
                var result = new JsonResultModel();
                var res = _aliyunService.GetAliyunTSTAccessKey();
                result.Data = res.ToJson();
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

    }
}
