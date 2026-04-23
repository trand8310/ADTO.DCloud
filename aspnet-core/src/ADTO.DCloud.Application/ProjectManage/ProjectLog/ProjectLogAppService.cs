using System;
using System.Linq;
using ADTO.DCloud.Project;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.ProjectManage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;

namespace ADTO.DCloud.ProjectManage
{
    /// <summary>
    /// 项目日志相关操作
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Project)]
    public class ProjectLogAppService : DCloudAppServiceBase, IProjectLogAppService
    {
        private readonly IRepository<ProjectLog, Guid> _logRepository;
        private readonly IRepository<User, Guid> _userRepository;
        public ProjectLogAppService(IRepository<ProjectLog, Guid> logRepository, IRepository<User, Guid> userRepository)
        {
            _logRepository = logRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 添加项目日志信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateProjectLogAsync(CreateProjectLogDto input)
        {
            var info = ObjectMapper.Map<ProjectLog>(input);
            await _logRepository.InsertAsync(info);
        }

        /// <summary>
        /// 项目日志分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectLogDto>> GetProjectLogPageList(PagedProjectLogResultRequestDto input)
        {
            var query = _logRepository.GetAll()
                         .Where(p => p.ProjectId == input.ProjectId)
                         .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.OperateType.Contains(input.KeyWord))
                         .WhereIf(input.StartDate.HasValue, p => p.CreationTime >= input.StartDate)
                         .WhereIf(input.EndDate.HasValue, p => p.CreationTime <= input.EndDate.Value.AddDays(1))
                         .Join(this._userRepository.GetAll(), log => log.CreatorUserId, user => user.Id,
                            (follow, user) => new
                            {
                                logInfo = follow,
                                userInfo = user
                            });
            query = query.OrderByDescending(p => p.logInfo.CreationTime);
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            var list = new List<ProjectLogDto>();
            foreach (var item in items)
            {
                var dto = ObjectMapper.Map<ProjectLogDto>(item.logInfo);
                //操作人
                dto.CreatorUserName = item.userInfo.Name;
                list.Add(dto);
            }
            return new PagedResultDto<ProjectLogDto>(totalCount, list);
        }
    }
}

