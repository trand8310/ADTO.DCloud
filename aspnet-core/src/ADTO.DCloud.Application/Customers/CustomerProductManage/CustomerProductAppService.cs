using ADTO.DCloud.Authorization;
using ADTO.DCloud.Customers.CustomerLogManage;
using ADTO.DCloud.Customers.CustomerLogManage.Dto;
using ADTO.DCloud.Customers.CustomerProductManage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Customers.CustomerProductManage
{
    /// <summary>
    /// 客户产品相关（手机端单独操作，和PC不同）
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers)]
    public class CustomerProductAppService : DCloudAppServiceBase, ICustomerProductAppService
    {
        private readonly IRepository<CustomerProduct, Guid> _customerProduct;
        private readonly ICustomerLogAppService _logAppService;
        public CustomerProductAppService(IRepository<CustomerProduct, Guid> customerProduct, ICustomerLogAppService logAppService)
        {
            _customerProduct = customerProduct;
            _logAppService = logAppService;
        }

        /// <summary>
        /// 添加客户产品信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateCustomerProductAsync(CreateCustomerProductDto input)
        {
            var info = ObjectMapper.Map<CustomerProduct>(input);
            await _customerProduct.InsertAsync(info);
            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = input.CustomerId, OperateType = "新增客户产品", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 修改客户产品
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        public async Task UpdateCustomerProductAsync(CreateCustomerProductDto input)
        {
            var info = this._customerProduct.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            //转换一下，否则其它字段也会置空
            await _customerProduct.UpdateAsync(info);

            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = input.CustomerId, OperateType = "修改客户产品", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的客户产品资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteCustomerProductAsync(EntityDto<Guid> input)
        {
            var info = await this._customerProduct.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            await _customerProduct.DeleteAsync(input.Id);

            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = info.CustomerId, OperateType = "删除客户产品", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取客户产品分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CustomerProductsDto>> GetCustomerProductPageList(PagedProductResultRequestDto input)
        {
            var query = _customerProduct.GetAll()
                 .Where(p => p.CustomerId == input.CustomerId);

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;
            //var list = items.Select(item =>
            //{
            //    var dto = ObjectMapper.Map<CustomerProductsDto>(item);
            //    return dto;
            //}).ToList();
            return new PagedResultDto<CustomerProductsDto>(totalCount, ObjectMapper.Map<List<CustomerProductsDto>>(items));
        }

        /// <summary>
        /// 获取指定客户产品详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CustomerProductsDto> GetCustomerProductByIdAsync(EntityDto<Guid> input)
        {
            var  info = await _customerProduct.GetAllIncluding(p => p.Customer).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var customerDto = ObjectMapper.Map<CustomerProductsDto>(info);
            customerDto.CustomerName = info.Customer.Name;
            customerDto.CustomerCode = info.Customer.Code;
            return customerDto;
        }
    }
}

