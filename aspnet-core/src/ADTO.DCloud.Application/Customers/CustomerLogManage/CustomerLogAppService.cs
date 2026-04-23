using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Customers.CustomerLogManage.Dto;

namespace ADTO.DCloud.Customers.CustomerLogManage
{
    /// <summary>
    /// 客户日志相关方法
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers)]
    public class CustomerLogAppService : DCloudAppServiceBase, ICustomerLogAppService
    {
        private readonly IRepository<CustomerLogs, Guid> _logRepository;
        private readonly IRepository<User, Guid> _userRepository;
        public CustomerLogAppService(IRepository<CustomerLogs, Guid> logRepository, IRepository<User, Guid> userRepository)
        {
            _logRepository = logRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 添加客户日志信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateCustomerLogAsync(CreateCustomerLogDto input)
        {
            var info = ObjectMapper.Map<CustomerLogs>(input);
            await _logRepository.InsertAsync(info);
        }

        /// <summary>
        /// 客户日志分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<CustomerLogDto>> GetCustomerLogPageList(PagedCustomerLogResultRequestDto input)
        {
            var query = _logRepository.GetAll()
                         .Where(p => p.CustomerId == input.CustomerId)
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

            var list = new List<CustomerLogDto>();
            foreach (var item in items)
            {
                var dto = ObjectMapper.Map<CustomerLogDto>(item.logInfo);
                //操作人
                dto.CreatorUserName = item.userInfo.Name;
                list.Add(dto);
            }
            return new PagedResultDto<CustomerLogDto>(totalCount, list);
        }
    }
}

