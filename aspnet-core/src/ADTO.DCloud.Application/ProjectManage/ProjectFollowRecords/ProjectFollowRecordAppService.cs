using ADTOSharp;
using System;
using System.IO;
using ADTOSharp.UI;
using System.Linq;
using ADTO.DCloud.Project;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.ProjectManage.Dto;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Linq.Extensions;
using ADTO.DCloud.Authorization.Users.Profile;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using ADTO.DCloud.Media.UploadFiles;
using ADTO.DCloud.ProjectManage.ProjectFollowRecords.Dto;
using Newtonsoft.Json;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;

namespace ADTO.DCloud.ProjectManage.ProjectFollowRecords
{
    /// <summary>
    /// 项目跟进记录相关操作
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Project_Follow)]
    public class ProjectFollowRecordAppService : DCloudAppServiceBase, IProjectFollowRecordAppService
    {
        private readonly IRepository<ProjectFollowRecord, Guid> _projectFollowRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IProfileAppService _profileAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IUploadFileAppService _uploadFileAppService;
        private readonly IProjectLogAppService _logAppService;
        public ProjectFollowRecordAppService(IRepository<ProjectFollowRecord, Guid> projectFollowRepository
            , IRepository<User, Guid> userRepository
            , IWebHostEnvironment webHostEnvironment
            , IGuidGenerator guidGenerator
            , IUploadFileAppService uploadFileAppService
            , IProjectLogAppService logAppService
            , IProfileAppService profileAppService)
        {
            _projectFollowRepository = projectFollowRepository;
            _userRepository = userRepository;
            _profileAppService = profileAppService;
            _webHostEnvironment = webHostEnvironment;
            _guidGenerator = guidGenerator;
            _logAppService = logAppService;
            _uploadFileAppService = uploadFileAppService;
        }

        #region
        ///// <summary>
        ///// 添加项目跟进记录
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public async Task CreateProjectFollowAsync(CreateProjectFollowRecordDto input)
        //{
        //    var info = ObjectMapper.Map<ProjectFollowRecord>(input);
        //    //有附件方式
        //    if (input.FileTokens?.Count > 0)
        //    {
        //        input.FolderId = _guidGenerator.Create();

        //        var entityId = await _projectFollowRepository.InsertAndGetIdAsync(info);

        //        if (input.FileTokens?.Count > 0)
        //        {
        //            await _uploadFileAppService.UploadFileAsync(new Media.UploadFiles.Dto.UploadSaveFileDto()
        //            {
        //                FolderId = input.FolderId,
        //                FileTokens = input.FileTokens,
        //                ProjectId = input.ProjectId,
        //                EntityId = entityId
        //            });
        //        }
        //    }
        //    else
        //    {
        //        await _projectFollowRepository.InsertAndGetIdAsync(info);
        //    }
        //}

        ///// <summary>
        ///// 修改项目跟进记录
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public async Task UpdateProjectFollowAsync(CreateProjectFollowRecordDto input)
        //{
        //    var info = this._projectFollowRepository.Get(input.Id.Value);
        //    if (info == null)
        //    {
        //        throw new UserFriendlyException("操作失败，记录不存在！");
        //    }

        //    //如果之前就有图片
        //    if (info.FolderId.HasValue && info.FolderId != Guid.Empty)
        //    {
        //        if (input.UploadFilesDtos?.Count > 0)
        //        {
        //            //校验是否删除了之前的图片
        //            //原本所有的图片
        //            var existingFiles = await this._uploadFileAppService.GetFileListByFolderId(input.FolderId.Value);

        //            // 找出被删除的文件
        //            var deletedFileIds = existingFiles
        //                .ExceptBy(input.UploadFilesDtos.Select(f => f.Id), f => f.Id)
        //                .Select(f => f.Id)
        //                .ToList();

        //            //批量删除
        //            if (deletedFileIds.Count > 0)
        //            {
        //                await _uploadFileAppService.DeleteUploadFileListAsync(deletedFileIds);
        //            }
        //        }
        //    }
        //    //新增了图片
        //    if (input.FileTokens?.Count > 0)
        //    {
        //        if (!(info.FolderId.HasValue && info.FolderId != Guid.Empty))
        //        {
        //            input.FolderId = _guidGenerator.Create();
        //        }

        //        await _uploadFileAppService.UploadFileAsync(new Media.UploadFiles.Dto.UploadSaveFileDto()
        //        {
        //            FolderId = info.FolderId,
        //            FileTokens = input.FileTokens,
        //            ProjectId = input.ProjectId,
        //            EntityId = input.Id.Value
        //        });
        //    }


        //    ObjectMapper.Map(input, info);
        //    await _projectFollowRepository.UpdateAsync(info);

        //}

        #endregion

        /// <summary>
        /// 添加项目跟进记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateProjectFollowAsync(CreateProjectFollowRecordDto input)
        {
            var info = ObjectMapper.Map<ProjectFollowRecord>(input);
            //存在附件
            if (input.UploadFilesDtos?.Count > 0)
            {
                info.FolderId = _guidGenerator.Create();
                var entityId = await _projectFollowRepository.InsertAndGetIdAsync(info);

                List<string> FileTokensList = new List<string>();
                foreach (var item in input.UploadFilesDtos)
                {
                    if (!string.IsNullOrWhiteSpace(item.FileToken))
                    {
                        FileTokensList.Add(item.FileToken);
                    }
                }
                if (FileTokensList.Count > 0)
                {
                    await _uploadFileAppService.UploadFileAsync(new Media.UploadFiles.Dto.UploadSaveFileDto()
                    {
                        FolderId = info.FolderId,
                        FileTokens = FileTokensList,
                        ProjectId = input.ProjectId,
                        EntityId = entityId
                    });
                }
            }
            else
            {
                await _projectFollowRepository.InsertAndGetIdAsync(info);
            }
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.ProjectId, OperateType = "新增跟进记录", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 修改项目跟进记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateProjectFollowAsync(CreateProjectFollowRecordDto input)
        {
            if (input?.Id == null)
            {
                throw new UserFriendlyException("操作失败，参数错误！");
            }
            var info = this._projectFollowRepository.Get(input.Id.Value);
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            if (input.UploadFilesDtos?.Count > 0)
            {
                //原本是有图片，校验是否有删除部分
                if (info.FolderId.HasValue && info.FolderId != Guid.Empty)
                {
                    //校验是否删除了之前的图片
                    //原本所有的图片
                    var existingFiles = await this._uploadFileAppService.GetFileListByFolderId(info.FolderId.Value);

                    // 找出被删除的文件
                    var deletedFileIds = existingFiles
                        .ExceptBy(input.UploadFilesDtos.Select(f => f.FileId), f => f.Id)
                        .Select(f => f.Id)
                        .ToList();

                    //批量删除
                    if (deletedFileIds.Count > 0)
                    {
                        await _uploadFileAppService.DeleteUploadFileListAsync(deletedFileIds);
                    }
                }

                var newFileToken = input.UploadFilesDtos.Where(p => !string.IsNullOrWhiteSpace(p.FileToken)).Select(p => p.FileToken).ToList();
                //新增了图片
                if (newFileToken?.Count > 0)
                {
                    if (!(info.FolderId.HasValue && info.FolderId != Guid.Empty))
                    {
                        input.FolderId = _guidGenerator.Create();
                    }

                    await _uploadFileAppService.UploadFileAsync(new Media.UploadFiles.Dto.UploadSaveFileDto()
                    {
                        FolderId = input.FolderId.Value,
                        FileTokens = newFileToken,
                        ProjectId = input.ProjectId,
                        EntityId = input.Id.Value
                    });
                }
            }
            //删除所有
            else
            {
                if (info.FolderId.HasValue && info.FolderId != Guid.Empty)
                {
                    await _uploadFileAppService.DeleteUploadFileByFolderIdAsync(info.FolderId.Value);
                }
            }

            ObjectMapper.Map(input, info);
            await _projectFollowRepository.UpdateAsync(info);

            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.ProjectId, OperateType = "修改跟进记录", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的项目跟进记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteProjectFollowAsync(EntityDto<Guid> input)
        {
            var info = await this._projectFollowRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            var loginUser = ADTOSharpSession.UserId.Value;
            if (info.FollowUserId != loginUser)
            {
                throw new UserFriendlyException("删除失败，只有所属者才允许删除");
            }
            await _projectFollowRepository.DeleteAsync(input.Id);
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = info.ProjectId, OperateType = "删除跟进记录", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取项目跟进记录分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectFollowRecordDto>> GetProjectFollowPageList(PagedProjectFollowResultRequestDto input)
        {
            var query = _projectFollowRepository.GetAll().Where(p => p.ProjectId == input.ProjectId)
                        .Join(this._userRepository.GetAll(), follow => follow.FollowUserId, user => user.Id,
                        (follow, user) => new
                        {
                            followInfo = follow,
                            userInfo = user
                        });
            query = query.OrderByDescending(p => p.followInfo.FollowTime);
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            var list = new List<ProjectFollowRecordDto>();
            foreach (var item in items)
            {
                var dto = ObjectMapper.Map<ProjectFollowRecordDto>(item.followInfo);
                dto.FollowUserName = item.userInfo.Name;
                //跟进人图像
                var imageUser = await _profileAppService.GetProfilePictureByUser(item.followInfo.FollowUserId);

                if (string.IsNullOrWhiteSpace(imageUser.ProfilePicture))
                {
                    byte[] defaultImageBytes = File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, "Common", "Images", "default-profile-picture.png"));
                    dto.FollowProfilePicture = Convert.ToBase64String(defaultImageBytes); // string
                }
                else
                {
                    dto.FollowProfilePicture = imageUser.ProfilePicture;
                }
                list.Add(dto);
            }
            return new PagedResultDto<ProjectFollowRecordDto>(totalCount, list);
        }

        /// <summary>
        /// 获取指定进记录详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProjectFollowRecordDto> GetProjectFollowByIdAsync(EntityDto<Guid> input)
        {
            var info = await _projectFollowRepository.GetAllIncluding(p => p.ProjectInfo).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var InfoDto = ObjectMapper.Map<ProjectFollowRecordDto>(info);
            InfoDto.FollowUserName = (await this._userRepository.GetAll().Where(p => p.Id == InfoDto.FollowUserId).FirstOrDefaultAsync())?.Name;

            if (info.FolderId.HasValue && info.FolderId != Guid.Empty)
            {
                var uploadFileList = await this._uploadFileAppService.GetFileUrlListByFolderId(info.FolderId.Value);
                InfoDto.UploadFilesDtos = uploadFileList.Select(item =>
                {
                    ProjectUploadFilesDto uploadFilesDto = new ProjectUploadFilesDto()
                    {
                        FileId = item.Id,
                        Url = item.FullAddress,
                        Name = item.FileName
                    };
                    return uploadFilesDto;
                }).ToList();
            }

            InfoDto.ProjectName = info.ProjectInfo.Name;
            InfoDto.ProjectCode = info.ProjectInfo.Code;
            return InfoDto;
        }
    }
}

