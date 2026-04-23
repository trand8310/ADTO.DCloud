using System;
using System.Linq;
using ADTOSharp.UI;
using Newtonsoft.Json;
using ADTO.DCloud.Project;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using ADTO.DCloud.ProjectManage.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.ProjectManage.ProjectContact
{
    /// <summary>
    /// 项目联系人相关接口
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Project_Contact)]
    public class ProjectContactAppService : DCloudAppServiceBase, IProjectContactAppService
    {
        private readonly IRepository<ProjectContacts, Guid> _contactRepository;
        private readonly IProjectLogAppService _logAppService;
        public ProjectContactAppService(IRepository<ProjectContacts, Guid> contactRepository, IProjectLogAppService logAppService)
        {
            _contactRepository = contactRepository;
            _logAppService = logAppService;
        }

        /// <summary>
        /// 添加项目联系人信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateProjectContactAsync(CreateProjectContactDto input)
        {
            var info = ObjectMapper.Map<ProjectContacts>(input);
            await _contactRepository.InsertAsync(info);
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.ProjectId, OperateType = "新增联系人", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 修改项目联系人资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateProjectContactAsync(CreateProjectContactDto input)
        {
            var info = this._contactRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            //转换一下，否则其它字段也会置空
            await _contactRepository.UpdateAsync(info);
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.ProjectId, OperateType = "修改联系人", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的项目联系人资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteProjectContactAsync(EntityDto<Guid> input)
        {
            var info = await this._contactRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            var countInfo = await this._contactRepository.CountAsync(p => p.ProjectId == info.ProjectId);
            if (countInfo <= 1)
            {
                throw new UserFriendlyException("操作失败，联系人记录必须存在一条记录！");
            }
            await _contactRepository.DeleteAsync(input.Id);
            await _logAppService.CreateProjectLogAsync(new CreateProjectLogDto() { ProjectId = input.Id, OperateType = "删除联系人", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取项目联系人分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectContactsDto>> GetProjectContactPageList(PagedProjectContactResultRequestDto input)
        {
            var query = _contactRepository.GetAll()
                 .Where(p => p.ProjectId == input.ProjectId);

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<ProjectContactsDto>(item);
                return dto;
            }).ToList();
            return new PagedResultDto<ProjectContactsDto>(totalCount, list);
        }

        /// <summary>
        /// 获取指定项目联系人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProjectContactsDto> GetProjectContactByIdAsync(EntityDto<Guid> input)
        {
            var info = await _contactRepository.GetAllIncluding(p => p.ProjectInfo).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var projectDto = ObjectMapper.Map<ProjectContactsDto>(info);
            projectDto.ProjectName = info.ProjectInfo.Name;
            projectDto.ProjectCode = info.ProjectInfo.Code;

            return projectDto;
        }
    }
}

