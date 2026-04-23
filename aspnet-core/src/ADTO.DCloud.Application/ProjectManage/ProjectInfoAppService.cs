using System;
using System.Linq;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Project;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using System.Threading.Tasks;
using ADTO.DCloud.ProjectManage.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.EntityFrameworkCore.Repositories;
using Newtonsoft.Json;
using ADTO.DCloud.DataAuthorizes;
using ADTOSharp;
using ADTO.DCloud.Media.UploadFiles;

namespace ADTO.DCloud.ProjectManage
{
    /// <summary>
    /// 项目相关接口
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Project)]
    public class ProjectInfoAppService : DCloudAppServiceBase, IProjectInfoAppService
    {
        #region Fields
        private readonly IRepository<ProjectInfo, Guid> _projectInfoRepository;
        private readonly IRepository<ProjectContacts, Guid> _projectContactsRepository;
        private readonly IRepository<ProjectContract, Guid> _projectContractRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IProjectLogAppService _logAppService;
        private readonly DataFilterService _dataAuthorizesApp;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IUploadFileAppService _uploadFileAppService;
        #endregion

        #region Ctor
        public ProjectInfoAppService(
            IRepository<ProjectInfo, Guid> projectInfoRepository
             , IRepository<User, Guid> userRepository
             , IProjectLogAppService logAppService
             , DataFilterService dataAuthorizesApp
            , IGuidGenerator guidGenerator
            , IRepository<ProjectContract, Guid> projectContractRepository
            , IUploadFileAppService uploadFileAppService
            , IRepository<ProjectContacts, Guid> projectContactsRepository)
        {
            _projectInfoRepository = projectInfoRepository;
            _projectContactsRepository = projectContactsRepository;
            _userRepository = userRepository;
            _projectContractRepository = projectContractRepository;
            _logAppService = logAppService;
            _dataAuthorizesApp = dataAuthorizesApp;
            _guidGenerator = guidGenerator;
            _uploadFileAppService = uploadFileAppService;
        }
        #endregion
        /// <summary>
        /// 生成项目编码(PJ+年月日+四位数自增数)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task<string> GenerateProjectCode()
        {
            string prefix = "PJ";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            var maxCode = await _projectInfoRepository.GetAll()
            .Where(c => c.Code.StartsWith(prefix + datePart))
            .Select(c => c.Code)
            .OrderByDescending(code => code)
            .FirstOrDefaultAsync();

            int newNumber = 1;
            if (maxCode != null)
            {
                string numberPart = maxCode.Substring(prefix.Length + datePart.Length);
                if (!int.TryParse(numberPart, out int parsedNumber))
                {
                    throw new UserFriendlyException($"无效的编码格式: {maxCode}");
                }
                newNumber = parsedNumber + 1;
            }
            return $"{prefix}{datePart}{newNumber:D4}"; // 4位数字
        }

        /// <summary>
        /// 添加项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Project_Create)]
        public async Task CreateProjectInfoAsync(CreateProjectInfoDto input)
        {
            var existInfo = await this._projectInfoRepository.GetAll().Where(p => p.Name == input.Name).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,项目名称已存在！");
            }
            //保存信息表
            var info = ObjectMapper.Map<ProjectInfo>(input);
            info.Code = await GenerateProjectCode();

            //存在附件
            if (input.UploadFilesDtos?.Count > 0)
            {
                info.FolderId = _guidGenerator.Create();

                info.Id = await _projectInfoRepository.InsertAndGetIdAsync(info);

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
                        ProjectId = info.Id,
                        EntityId = info.Id,
                    });
                }
            }
            else
            {
                info.Id = await _projectInfoRepository.InsertAndGetIdAsync(info);
            }

            CurrentUnitOfWork.SaveChanges();
            //保存项目联系人记录
            if (input.ProjectContacts?.Count > 0)
            {
                foreach (var item in input.ProjectContacts)
                {
                    var contactInfo = ObjectMapper.Map<ProjectContacts>(item);
                    contactInfo.ProjectId = input.Id.Value;
                    await _projectContactsRepository.InsertAsync(contactInfo);
                }
            }
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = info.Id, OperateType = "新增项目", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 修改项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Project_Edit)]
        public async Task UpdateProjectInfoAsync(CreateProjectInfoDto input)
        {
            // 参数校验
            if (input.Id == null || input.Id == Guid.Empty)
            {
                throw new ArgumentNullException("参数错误");
            }
            var info = await this._projectInfoRepository.GetAsync(input.Id.Value);
            if (info == null)
            {
                throw new UserFriendlyException("项目不存在");
            }
            var existInfo = await this._projectInfoRepository.GetAll().Where(p => (p.Name == input.Name) && p.Id != input.Id).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,项目名称已存在！");
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
                        ProjectId = info.Id,
                        EntityId = info.Id
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

            //修改项目主体信息
            await _projectInfoRepository.UpdateAsync(info);

            //处理联系人记录
            await ProcessContactsAsync(info.Id, input.ProjectContacts);

            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.Id.Value, OperateType = "修改项目", DataDetail = JsonConvert.SerializeObject(input) });

        }
        /// <summary>
        /// 处理项目联系人
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="contacts"></param>
        /// <returns></returns>
        private async Task ProcessContactsAsync(Guid projectId, List<CreateProjectContactDto> contacts)
        {
            foreach (var item in contacts)
            {
                //修改
                if (item.Id.HasValue && item.Id.Value != Guid.Empty)
                {
                    var ContactsInfo = this._projectContactsRepository.Get(item.Id.Value);
                    ObjectMapper.Map(item, ContactsInfo);

                    //转换一下，否则其它字段也会置空
                    await _projectContactsRepository.UpdateAsync(ContactsInfo);
                }
                //新增
                else
                {
                    var contactInfo = ObjectMapper.Map<ProjectContacts>(item);
                    contactInfo.ProjectId = projectId;
                    await _projectContactsRepository.InsertAsync(contactInfo);
                }
            }
            // 删除不在输入中的联系人记录
            var updatedContactIds = contacts.Select(c => c.Id).ToList();
            await this._projectContactsRepository.BatchDeleteAsync(p => p.ProjectId == projectId && !updatedContactIds.Contains(p.Id));

            if (contacts.Count == 0)
            {
                //删除所有
                await this._projectContactsRepository.BatchDeleteAsync(p => p.ProjectId == projectId);
            }
        }

        /// <summary>
        /// 获取项目分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        [DataAuthPermission("GetProjectPageList")]
        public async Task<PagedResultDto<ProjectInfoDto>> GetProjectPageList(PagedProjectInfoRequestDto input)
        {
            var query = from project in this._projectInfoRepository.GetAll()
                        join user in this._userRepository.GetAll() on project.CreatorUserId equals user.Id
                        select new
                        {
                            Project = project,
                            User = user,
                            //如果有合同，就为ture
                            HasContract = this._projectContractRepository.GetAll().Any(contract => contract.ProjectId == project.Id)
                        };
            if (!string.IsNullOrWhiteSpace(input.KeyWord))
            {
                query = query.Where(p => p.Project.Name.Contains(input.KeyWord) || p.Project.Code == input.KeyWord);
            }
            if (input.StartDate.HasValue)
            {
                query = query.Where(p => p.Project.CreationTime >= input.StartDate);
            }
            if (input.EndDate.HasValue)
            {
                query = query.Where(p => p.Project.CreationTime <= input.EndDate.Value.AddDays(1));
            }
            if (input.MinAmount > 0)
            {
                query = query.Where(p => p.Project.Amount >= input.MinAmount);
            }
            if (input.MaxAmount > 0)
            {
                query = query.Where(p => p.Project.Amount <= input.MaxAmount);
            }
            // 添加IsSign过滤条件
            if (input.IsSign.HasValue)
            {
                query = query.Where(p => p.HasContract == input.IsSign.Value);
            }
            //数据权限
            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetProjectPageList)));
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            var totalCount = await query.CountAsync();
            //根据更新日期排序
            var items = await query.OrderByDescending(p => p.Project.CreationTime).PageBy(input).ToListAsync();
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<ProjectInfoDto>(item.Project);
                dto.CreatorUserName = item.User?.Name;
                //是否签订合同
                // dto.IsSign = this._projectContractRepository.GetAll().Any(p => p.ProjectId == dto.Id);
                dto.IsSign = item.HasContract;

                //联系人电话（第一条联系人）
                dto.FirstContactPhone = this._projectContactsRepository.GetAll().Where(p => p.ProjectId == item.Project.Id).OrderBy(p => p.CreationTime).Select(p => p.Phone).FirstOrDefault();
                return dto;
            }).ToList();
            return new PagedResultDto<ProjectInfoDto>(totalCount, list);
        }
          /// <summary>
        /// 获取指定项目详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProjectDetailShowDto> GetProjectByIdAsync(EntityDto<Guid> input)
        {
            var query = from project in this._projectInfoRepository.GetAll()
                        where project.Id == input.Id
                        join u in this._userRepository.GetAll() on project.CreatorUserId equals u.Id into u_d
                        from creatorUser in u_d.DefaultIfEmpty()
                        select new
                        {
                            project,
                            creatorUserName = creatorUser.Name,

                        };
            var info = await query.FirstOrDefaultAsync();
            var projectDto = ObjectMapper.Map<ProjectDetailShowDto>(info.project);
            //创建人
            projectDto.CreatorUserName = info.creatorUserName;

            //读取联系人
            var contactList = await _projectContactsRepository.GetAll().Where(p => p.ProjectId == input.Id).ToListAsync();
            projectDto.ProjectContacts = ObjectMapper.Map<List<ProjectContactsDto>>(contactList);

            //详情附件读取（手机端中有客户新增有附件）
            if (projectDto.FolderId.HasValue && projectDto.FolderId != Guid.Empty)
            {
                var uploadFileList = await this._uploadFileAppService.GetFileUrlListByFolderId(projectDto.FolderId.Value);
                projectDto.UploadFilesDtos = uploadFileList.Select(item =>
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

            return projectDto;
        }

        /// <summary>
        /// 是否存在相同的项目名称
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProjectInfoDto> IsExistProjectName(ProjectExistInfoDto input)
        {
            // 参数校验
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException("名称不能为空");
            }
            // 构建查询条件
            var query = _projectInfoRepository.GetAll();
            if (input.Id.HasValue && input.Id.Value != Guid.Empty)
            {
                // 修改场景：排除当前记录
                query = query.Where(p => p.Id != input.Id);
            }
            // 检查名称
            if (!string.IsNullOrWhiteSpace(input.Name))
            {
                query = query.Where(p => p.Name == input.Name);
            }
            var existingInfo = await query.FirstOrDefaultAsync();
            if (existingInfo != null)
            {
                throw new UserFriendlyException("项目名称已存在");
            }
            return ObjectMapper.Map<ProjectInfoDto>(existingInfo);
        }
    }
}

