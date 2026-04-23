using System;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Customers.CustomerContactManage.Dto;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using ADTO.DCloud.Customers.CustomerLogManage;
using ADTO.DCloud.Customers.CustomerLogManage.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ADTO.DCloud.Customers.CustomerContactManage
{
    /// <summary>
    /// 客户联系人相关方法
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers_Contact)]
    public class CustomerContactAppService : DCloudAppServiceBase, ICustomerContactAppService
    {
        private readonly IRepository<CustomerContacts, Guid> _contactRepository;
        private readonly ICustomerLogAppService _logAppService;
        public CustomerContactAppService(IRepository<CustomerContacts, Guid> contactRepository, ICustomerLogAppService logAppService)
        {
            _contactRepository = contactRepository;
            _logAppService = logAppService;
        }
        /// <summary>
        /// 添加客户联系人信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateCustomerContactAsync(CreateCustomerContactsDto input)
        {
            var info = ObjectMapper.Map<CustomerContacts>(input);
            await _contactRepository.InsertAsync(info);
            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = input.CustomerId, OperateType = "新增联系人", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 修改客户联系人资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        public async Task UpdateCustomerContactAsync(CreateCustomerContactsDto input)
        {
            var info = this._contactRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            //转换一下，否则其它字段也会置空
            await _contactRepository.UpdateAsync(info);

            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = input.CustomerId, OperateType = "修改联系人", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的客户联系人资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteCustomerContactAsync(EntityDto<Guid> input)
        {
            var info = await this._contactRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            var countInfo = await this._contactRepository.CountAsync(p => p.CustomerId == info.CustomerId);
            if (countInfo <= 1)
            {
                throw new UserFriendlyException("操作失败，联系人记录必须存在一条记录！");
            }
            await _contactRepository.DeleteAsync(input.Id);

            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = info.CustomerId, OperateType = "删除联系人", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取客户联系人分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CustomerContactsDto>> GetCustomerContactPageList(PagedContactResultRequestDto input)
        {
            var query = _contactRepository.GetAll()
                 .Where(p => p.CustomerId == input.CustomerId);

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;
            //var list = items.Select(item =>
            //{
            //    var dto = ObjectMapper.Map<CustomerContactsDto>(item);
            //    return dto;
            //}).ToList();
            return new PagedResultDto<CustomerContactsDto>(totalCount, ObjectMapper.Map<List<CustomerContactsDto>>(items));
        }

        /// <summary>
        /// 获取指定客户联系人详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CustomerContactsDto> GetCustomerContactByIdAsync(EntityDto<Guid> input)
        {
            var info = await _contactRepository.GetAllIncluding(p => p.Customer).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var customerDto = ObjectMapper.Map<CustomerContactsDto>(info);
            customerDto.CustomerName = info.Customer.Name;
            customerDto.CustomerCode = info.Customer.Code;
            return customerDto;
        }


    }
}

