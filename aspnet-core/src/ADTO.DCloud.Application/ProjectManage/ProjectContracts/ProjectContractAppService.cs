using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.FormScheme.Model;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.UploadFiles;
using ADTO.DCloud.Project;
using ADTO.DCloud.ProjectManage.Dto;
using ADTO.DCloud.ProjectManage.ProjectContracts.Dto;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.ProjectManage.ProjectContracts
{
    /// <summary>
    /// 项目合同相关操作
    /// </summary>
    public class ProjectContractAppService : DCloudAppServiceBase, IProjectContractAppService, IWorkFlowMethod
    {
        private readonly IRepository<ProjectContract, Guid> _contractRepository;
        private readonly IProjectLogAppService _logAppService;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IUploadFileAppService _uploadFileAppService;
        private readonly DataFilterService _dataAuthorizesApp;
        public ProjectContractAppService(IRepository<ProjectContract, Guid> contractRepository
            , IRepository<User, Guid> userRepository

            , IGuidGenerator guidGenerator
            , IUploadFileAppService uploadFileAppService
             , DataFilterService dataAuthorizesApp
            , IProjectLogAppService logAppService)
        {
            _contractRepository = contractRepository;
            _userRepository = userRepository;
            _guidGenerator = guidGenerator;
            _logAppService = logAppService;
            _uploadFileAppService = uploadFileAppService;
            _dataAuthorizesApp = dataAuthorizesApp;
        }
        /// <summary>
        /// 生成项目合同编码(PJ+年月日+四位数自增数)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task<string> GenerateContractCode()
        {
            string prefix = "HT";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            var maxCode = await _contractRepository.GetAll()
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
        /// 添加项目合同记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProjectContract> CreateProjectContractAsync(CreateProjectContractDto input)
        {
            var info = ObjectMapper.Map<ProjectContract>(input);
            info.Code = await GenerateContractCode();
            //存在附件
            if (input.UploadFilesDtos?.Count > 0)
            {
                info.FolderId = _guidGenerator.Create();

                info.Id = await _contractRepository.InsertAndGetIdAsync(info);

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
                        EntityId = info.Id
                    });
                }
            }
            else
            {
                await _contractRepository.InsertAndGetIdAsync(info);
            }
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.ProjectId, OperateType = "新增项目合同", DataDetail = JsonConvert.SerializeObject(input) });

            return info;
        }

        /// <summary>
        /// 修改项目合同记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateProjectContractAsync(CreateProjectContractDto input)
        {
            if (input?.Id == null)
            {
                throw new UserFriendlyException("操作失败，参数错误！");
            }
            var info = this._contractRepository.Get(input.Id.Value);
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
            await _contractRepository.UpdateAsync(info);

            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.ProjectId, OperateType = "修改项目合同", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的项目合同记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteProjectContractAsync(EntityDto<Guid> input)
        {
            var info = await this._contractRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            var loginUser = ADTOSharpSession.UserId.Value;

            await _contractRepository.DeleteAsync(input.Id);
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = info.ProjectId, OperateType = "删除项目合同", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取项目合同记录分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        [DataAuthPermission("GetProjectContractPageList")]
        public async Task<PagedResultDto<ProjectContractDto>> GetProjectContractPageList(PagedProjectContractResultRequestDto input)
        {
            var query = _contractRepository.GetAllIncluding(p => p.ProjectInfo)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Code == input.KeyWord || p.PartyAName.Contains(input.KeyWord) || p.PartyBName.Contains(input.KeyWord))
                        .WhereIf(input.ProjectId.HasValue && input.ProjectId.Value != Guid.Empty, p => p.ProjectId == input.ProjectId)
                        //签约时间
                        .WhereIf(input.StartDate.HasValue, p => p.SigningTime >= input.StartDate)
                        .WhereIf(input.EndDate.HasValue, p => p.SigningTime <= input.EndDate.Value.AddDays(1))
                        .WhereIf(input.MinAmount.HasValue && input.MinAmount > 0, p => p.ContractAmount >= input.MinAmount)
                        .WhereIf(input.MaxAmount.HasValue && input.MinAmount > 0, p => p.ContractAmount <= input.MaxAmount)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectName), p => p.ProjectInfo.Name.Contains(input.ProjectName))
                        //审核状态
                        .WhereIf(input.AuditStatus.HasValue, p => p.AuditStatus == input.AuditStatus)
                        .Join(this._userRepository.GetAll(), contract => contract.CreatorUserId, user => user.Id,
                        (contract, user) => new
                        {
                            contract = contract,
                            userInfo = user
                        });

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetProjectContractPageList)));
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);
            query = query.OrderByDescending(p => p.contract.CreationTime);
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            var list = new List<ProjectContractDto>();
            foreach (var item in items)
            {
                var dto = ObjectMapper.Map<ProjectContractDto>(item.contract);
                dto.CreatorUserName = item.userInfo.Name;

                list.Add(dto);
            }
            return new PagedResultDto<ProjectContractDto>(totalCount, list);
        }

        /// <summary>
        /// 获取指定合同记录详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProjectContractDto> GetProjectContractByIdAsync(EntityDto<Guid> input)
        {
            var info = await _contractRepository.GetAllIncluding(p => p.ProjectInfo).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var InfoDto = ObjectMapper.Map<ProjectContractDto>(info);

            InfoDto.CreatorUserName = (await this._userRepository.GetAll().Where(p => p.Id == InfoDto.CreatorUserId).FirstOrDefaultAsync())?.Name;
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
            return InfoDto;
        }

        /// <summary>
        /// 修改审核状态
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAuditStatus(int AuditStatus, Guid Id)
        {
            await this._contractRepository.UpdateAsync(Id, async entity => { entity.AuditStatus = AuditStatus; });
        }
        /// <summary>
        /// 作废流程
        /// </summary>
        /// <returns></returns>
        public async Task CancelStatus(Guid Id)
        {
            await this._contractRepository.UpdateAsync(Id, async entity => { entity.AuditStatus = 3; });
        }
        #region 流程执行方法

        public Task Execute(WfMethodParameter parameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行流程新增表单信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ExecuteInsert(string data)
        {
            var dto = data.ToObject<CreateProjectContractDto>();
            await this.CreateProjectContractAsync(dto);
        }
        /// <summary>
        /// 修改流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ExecuteUpdate(string data)
        {
            var dto = data.ToObject<CreateProjectContractDto>();
            await this.UpdateProjectContractAsync(dto);
        }

        /// <summary>
        /// 删除流程表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ExecuteDelete(Guid id)
        {
            await this.DeleteProjectContractAsync(new EntityDto<Guid>() { Id = id });
        }
        #endregion

    }
}

