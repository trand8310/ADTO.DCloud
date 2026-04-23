using System;
using System.Linq;
using ADTOSharp.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Domain.Repositories;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Customers.CustomerShareManage.Dto;
using ADTO.DCloud.Customers.CustomerLogManage;
using ADTO.DCloud.Customers.CustomerLogManage.Dto;

namespace ADTO.DCloud.Customers.CustomerShareManage
{
    /// <summary>
    /// 客户分享相关操作
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers_Shared)]
    public class CustomerShareAppService : DCloudAppServiceBase, ICustomerShareAppService
    {
        private readonly IRepository<CustomerShareRecord, Guid> _shareRecordRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly ICustomerLogAppService _logAppService;
        public CustomerShareAppService(IRepository<CustomerShareRecord, Guid> shareRecordRepository
            , IRepository<Customer, Guid> customerRepository
            , ICustomerLogAppService logAppService
            , IRepository<User, Guid> userRepository)
        {
            _shareRecordRepository = shareRecordRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _logAppService = logAppService;
        }

        /// <summary>
        /// 添加客户分享记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateCustomerShareAsync(CreateCustomerShareRecordDto input)
        {
            if (input.ToUserIdList.Count <= 0)
            {
                throw new UserFriendlyException("请选择被分享人");
            }
            foreach (var item in input.ToUserIdList)
            {
                var info = ObjectMapper.Map<CustomerShareRecord>(input);
                info.ToUserId = item;
                //分享人=当前登录用户
                info.FromUserId = ADTOSharpSession.UserId.Value;
                await _shareRecordRepository.InsertAsync(info);
            }
            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = input.CustomerId, OperateType = "新增客户分享", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的客户分享记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteCustomerShareAsync(EntityDto<Guid> input)
        {
            var info = await this._shareRecordRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("删除失败，该记录不存在");
            }
            var loginUser = ADTOSharpSession.UserId.Value;
            if (info.FromUserId != loginUser)
            {
                throw new UserFriendlyException("删除失败，只有分享者才允许删除");
            }
            await _shareRecordRepository.DeleteAsync(input.Id);
            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = info.CustomerId, OperateType = "删除客户分享", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取客户跟进记录分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CustomerShareRecordDto>> GetCustomerSharePageList(PagedShareRecordResultRequestDto input)
        {
            var query = from share in this._shareRecordRepository.GetAllIncluding(p => p.Customer).Where(p => p.CustomerId == input.CustomerId)
                        join fromuser in this._userRepository.GetAll() on share.FromUserId equals fromuser.Id
                        join touser in this._userRepository.GetAll() on share.ToUserId equals touser.Id
                        select new { share, CustomerName = share.Customer.Name, fromUserName = fromuser.Name, toUserName = touser.Name };

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<CustomerShareRecordDto>(item.share);
                dto.FromUserName = item.fromUserName;
                dto.ToUserName = item.toUserName;
                dto.CustomerName = item.CustomerName;

                return dto;
            }).ToList();
            return new PagedResultDto<CustomerShareRecordDto>(totalCount, list);
        }
    }
}

