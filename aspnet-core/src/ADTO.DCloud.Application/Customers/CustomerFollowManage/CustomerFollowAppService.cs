using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Profile;
using ADTO.DCloud.Customers.CustomerFollowManage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using ADTO.DCloud.Customers.CustomerLogManage.Dto;
using Newtonsoft.Json;
using ADTO.DCloud.Customers.CustomerLogManage;
using ADTOSharp.UI;

namespace ADTO.DCloud.Customers.CustomerFollowManage
{
    /// <summary>
    /// 客户跟进记录操作方法
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers_Follow)]
    public class CustomerFollowAppService : DCloudAppServiceBase, ICustomerFollowAppService
    {
        private readonly IRepository<CustomerFollowRecord, Guid> _followReocrdRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IProfileAppService _profileAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICustomerLogAppService _logAppService;
        public CustomerFollowAppService(IRepository<CustomerFollowRecord, Guid> followReocrdRepository
            , IRepository<User, Guid> userRepository
            , IWebHostEnvironment webHostEnvironment
            , ICustomerLogAppService logAppService
            , IProfileAppService profileAppService)
        {
            _followReocrdRepository = followReocrdRepository;
            _userRepository = userRepository;
            _profileAppService = profileAppService;
            _webHostEnvironment = webHostEnvironment;
            _logAppService = logAppService;
        }

        /// <summary>
        /// 添加客户跟进记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateCustomerFollowAsync(CreateCustomerFollowRecordDto input)
        {
            var info = ObjectMapper.Map<CustomerFollowRecord>(input);
            await _followReocrdRepository.InsertAsync(info);

            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = info.CustomerId, OperateType = "新增跟进记录", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 修改客户跟进记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateCustomerFollowAsync(CreateCustomerFollowRecordDto input)
        {
            var info = this._followReocrdRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            //转换一下，否则其它字段也会置空
            await _followReocrdRepository.UpdateAsync(info);
            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = info.CustomerId, OperateType = "修改跟进记录", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 删除指定的客户跟进记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteCustomerFollowAsync(EntityDto<Guid> input)
        {
            var info = await this._followReocrdRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            var loginUser = ADTOSharpSession.UserId.Value;
            if (info.FollowUserId != loginUser)
            {
                throw new UserFriendlyException("删除失败，只有所属者才允许删除");
            }
            await _followReocrdRepository.DeleteAsync(input.Id);
            await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = info.CustomerId, OperateType = "删除跟进记录", DataDetail = JsonConvert.SerializeObject(input) });
        }

        /// <summary>
        /// 获取客户跟进记录分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CustomerFollowRecordDto>> GetCustomerFollowPageList(PagedFollowRecordResultRequestDto input)
        {
            var query = _followReocrdRepository.GetAll().Where(p => p.CustomerId == input.CustomerId)
                        .Join(this._userRepository.GetAll(), follow => follow.FollowUserId, user => user.Id,
                        (follow, user) => new
                        {
                            followInfo = follow,
                            userInfo = user
                        });
            query = query.OrderByDescending(p => p.followInfo.FollowTime);
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            var list = new List<CustomerFollowRecordDto>();
            foreach (var item in items)
            {
                var dto = ObjectMapper.Map<CustomerFollowRecordDto>(item.followInfo);
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

            return new PagedResultDto<CustomerFollowRecordDto>(totalCount, list);
        }

        /// <summary>
        /// 获取指定进记录详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CustomerFollowRecordDto> GetCustomerFollowByIdAsync(EntityDto<Guid> input)
        {
            var info = await _followReocrdRepository.GetAllIncluding(p => p.Customer).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var customerDto = ObjectMapper.Map<CustomerFollowRecordDto>(info);
            customerDto.FollowUserName = (await this._userRepository.GetAll().Where(p => p.Id == customerDto.FollowUserId).FirstOrDefaultAsync())?.Name;
            customerDto.CustomerName = info.Customer.Name;
            customerDto.CustomerCode = info.Customer.Code;

            return customerDto;
        }
    }
}

