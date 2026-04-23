using System;
using System.Linq;
using ADTOSharp.UI;
using Newtonsoft.Json;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using System.Threading.Tasks;
using ADTO.DCloud.Customers.Dto;
using System.Collections.Generic;
using ADTOSharp.Linq.Extensions;
using ADTO.DCloud.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTO.DCloud.Customers.CustomerContactManage.Dto;
using ADTO.DCloud.Customers.CustomerLogManage;
using ADTO.DCloud.Customers.CustomerLogManage.Dto;
using ADTO.DCloud.DataAuthorizes;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Http;
using ADTOSharp;
using ADTO.DCloud.Media.UploadFiles;
using ADTO.DCloud.ProjectManage.Dto;
using ADTO.DCloud.DataBase.Location;
using ADTO.DCloud.AreaBase;

namespace ADTO.DCloud.Customers;
/// <summary>
/// “客户管理”页面使用的应用程序服务。
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers)]
public class CustomerAppService : DCloudAppServiceBase, ICustomerAppService
{
    #region Fields
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly IRepository<Base_Country, Guid> _countryRepository;
    private readonly IRepository<CustomerContacts, Guid> _contactRepository;
    private readonly IRepository<CustomerProduct, Guid> _productRepository;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
    private readonly IRepository<CustomerFollowRecord, Guid> _followRepository;
    private readonly IRepository<CustomerShareRecord, Guid> _shareRepository;
    private readonly ICustomerLogAppService _logAppService;
    private readonly DataFilterService _dataAuthorizesApp;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IUploadFileAppService _uploadFileAppService;
    private readonly ILocationAppService _locationAppService;
    #endregion

    #region Ctor
    public CustomerAppService(
        IRepository<Customer, Guid> customerRepository
        , IRepository<Base_Country, Guid> countryRepository
        , IRepository<CustomerContacts, Guid> contactRepository
        , IRepository<CustomerProduct, Guid> productRepository
        , IRepository<User, Guid> userRepository
        , IRepository<CustomerFollowRecord, Guid> followRepository
        , IRepository<DataItemDetail, Guid> dataItemDetailRepository
        , DataFilterService dataAuthorizesApp
        , IRepository<CustomerShareRecord, Guid> shareRepository
        , ICustomerLogAppService logAppService
          , IUploadFileAppService uploadFileAppService
        , ILocationAppService locationAppService
        , IGuidGenerator guidGenerator
       )
    {
        _customerRepository = customerRepository;
        _countryRepository = countryRepository;
        _contactRepository = contactRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _dataItemDetailRepository = dataItemDetailRepository;
        _followRepository = followRepository;
        _logAppService = logAppService;
        _dataAuthorizesApp = dataAuthorizesApp;
        _shareRepository = shareRepository;
        _guidGenerator = guidGenerator;
        _uploadFileAppService = uploadFileAppService;
        _locationAppService = locationAppService;

    }
    #endregion

    #region Utilities

    /// <summary>
    /// 生成客户编码
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task<string> GenerateCustomerCode(Guid countryId)
    {
        var country = await _countryRepository.GetAsync(countryId);
        if (country == null)
        {
            throw new UserFriendlyException("国家信息不存在");
        }
        var maxCode = await _customerRepository.GetAll()
            //.Where(c => c.CountryId == countryId)
            .Select(c => c.Code)
            .OrderByDescending(code => code)
            .FirstOrDefaultAsync();

        int newNumber = 10001;
        if (maxCode != null)
        {
            if (!maxCode.StartsWith(country.Code) || !int.TryParse(maxCode.Substring(country.Code.Length), out int parsedNumber))
            {
                throw new UserFriendlyException($"无效的客户编码格式: {maxCode}");
            }
            newNumber = parsedNumber + 1;
        }
        return $"{country.Code}{newNumber:D5}";
    }

    /// <summary>
    /// 处理客户联系人
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="contacts"></param>
    /// <returns></returns>
    private async Task ProcessContactsAsync(Guid customerId, List<CreateCustomerContactsDto> contacts)
    {
        foreach (var item in contacts)
        {
            //修改
            if (item.Id.HasValue && item.Id.Value != Guid.Empty)
            {
                var ContactsInfo = this._contactRepository.Get(item.Id.Value);
                ObjectMapper.Map(item, ContactsInfo);

                //转换一下，否则其它字段也会置空
                await _contactRepository.UpdateAsync(ContactsInfo);
            }
            //新增
            else
            {
                var contactInfo = ObjectMapper.Map<CustomerContacts>(item);
                contactInfo.CustomerId = customerId;
                await _contactRepository.InsertAsync(contactInfo);
            }
        }
        // 删除不在输入中的联系人记录
        var updatedContactIds = contacts.Select(c => c.Id).ToList();
        await this._contactRepository.BatchDeleteAsync(p => p.CustomerId == customerId && !updatedContactIds.Contains(p.Id));

        if (contacts.Count == 0)
        {
            //删除所有
            await this._contactRepository.BatchDeleteAsync(p => p.CustomerId == customerId);
        }
    }

    /// <summary>
    /// 处理客户商品
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="products"></param>
    /// <returns></returns>
    private async Task ProcessProductAsync(Guid customerId, List<CustomerProductDto> products)
    {
        foreach (var item in products)
        {
            //修改
            if (item.Id.HasValue && item.Id.Value != Guid.Empty)
            {
                var ContactsInfo = this._productRepository.Get(item.Id.Value);
                ObjectMapper.Map(item, ContactsInfo);

                //转换一下，否则其它字段也会置空
                await _productRepository.UpdateAsync(ContactsInfo);
            }
            //新增
            else
            {
                var contactInfo = ObjectMapper.Map<CustomerProduct>(item);
                contactInfo.CustomerId = customerId;
                await _productRepository.InsertAsync(contactInfo);
            }
        }
        // 删除不在输入中的产品记录
        var updatedProductIds = products.Select(c => c.Id).ToList();
        await this._productRepository.BatchDeleteAsync(p => p.CustomerId == customerId && !updatedProductIds.Contains(p.Id));

        if (products.Count == 0)
        {
            //删除所有
            await this._productRepository.BatchDeleteAsync(p => p.CustomerId == customerId);
        }
    }

    #endregion

    /// <summary>
    /// 添加客户资料
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers_Create)]
    public async Task CreateCustomerAsync(CreateCustomerInputDto input)
    {
        var existInfo = await this._customerRepository.GetAll().Where(p => p.Name == input.Name || p.Email == input.Email).FirstOrDefaultAsync();
        if (existInfo != null)
        {
            throw new UserFriendlyException("保存失败,客户名称或邮箱已存在！");
        }
        //保存客户信息表
        var customer = ObjectMapper.Map<Customer>(input);
        customer.Code = await GenerateCustomerCode(input.CountryId);
        //所属人默认创建人
        customer.ResponsibilityUserId = ADTOSharpSession.UserId.Value;

        //存在附件
        if (input.UploadFilesDtos?.Count > 0)
        {
            customer.FolderId = _guidGenerator.Create();

            customer.Id = await _customerRepository.InsertAndGetIdAsync(customer);

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
                    FolderId = customer.FolderId,
                    FileTokens = FileTokensList,
                    ProjectId = customer.Id,
                    EntityId = customer.Id,
                });
            }
        }
        else
        {
            customer.Id = await _customerRepository.InsertAndGetIdAsync(customer);
        }
        // var createdCustomer = await _customerRepository.InsertAsync(customer);

        CurrentUnitOfWork.SaveChanges();
        //保存客户联系人记录
        if (input.CustomerContacts?.Count > 0)
        {
            foreach (var item in input.CustomerContacts)
            {
                var contactInfo = ObjectMapper.Map<CustomerContacts>(item);
                contactInfo.CustomerId = customer.Id;
                await _contactRepository.InsertAsync(contactInfo);
            }
        }
        // 保存客户产品记录
        if (input.CustomerProducts?.Count > 0)
        {
            foreach (var item in input.CustomerProducts)
            {
                var productInfo = ObjectMapper.Map<CustomerProduct>(item);
                productInfo.CustomerId = customer.Id;
                await _productRepository.InsertAsync(productInfo);
            }
        }
        await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = customer.Id, OperateType = "新增客户", DataDetail = JsonConvert.SerializeObject(input) });
    }

    /// <summary>
    /// 修改客户资料
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers_Edit)]
    public async Task UpdateCustomerAsync(CreateCustomerInputDto input)
    {
        // 参数校验
        if (input.Id == null || input.Id == Guid.Empty)
        {
            throw new ArgumentNullException("客户信息不能为空");
        }
        var customer = await this._customerRepository.GetAsync(input.Id.Value);
        if (customer == null)
        {
            throw new UserFriendlyException("客户不存在");
        }
        var existInfo = await this._customerRepository.GetAll().Where(p => (p.Name == input.Name || p.Email == input.Email) && p.Id != input.Id).FirstOrDefaultAsync();
        if (existInfo != null)
        {
            throw new UserFriendlyException("保存失败,客户名称或邮箱已存在！");
        }

        if (input.UploadFilesDtos?.Count > 0)
        {
            //原本是有图片，校验是否有删除部分
            if (customer.FolderId.HasValue && customer.FolderId != Guid.Empty)
            {
                //校验是否删除了之前的图片
                //原本所有的图片
                var existingFiles = await this._uploadFileAppService.GetFileListByFolderId(customer.FolderId.Value);

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
                if (!(customer.FolderId.HasValue && customer.FolderId != Guid.Empty))
                {
                    input.FolderId = _guidGenerator.Create();
                }

                await _uploadFileAppService.UploadFileAsync(new Media.UploadFiles.Dto.UploadSaveFileDto()
                {
                    FolderId = input.FolderId.Value,
                    FileTokens = newFileToken,
                    ProjectId = customer.Id,
                    EntityId = customer.Id
                });
            }
        }
        //删除所有
        else
        {
            if (customer.FolderId.HasValue && customer.FolderId != Guid.Empty)
            {
                await _uploadFileAppService.DeleteUploadFileByFolderIdAsync(customer.FolderId.Value);
            }
        }

        ObjectMapper.Map(input, customer);

        //修改客户主体信息
        await _customerRepository.UpdateAsync(customer);

        //处理客户联系人记录
        await ProcessContactsAsync(customer.Id, input.CustomerContacts);

        //处理客户产品
        await ProcessProductAsync(customer.Id, input.CustomerProducts);

        await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = customer.Id, OperateType = "修改客户", DataDetail = JsonConvert.SerializeObject(input) });
    }

    ///// <summary>
    ///// 获取文件路径
    ///// </summary>
    ///// <returns></returns>
    //private string GetRequestPath()
    //{
    //    return IocManager.Instance.Resolve<IHttpContextAccessor>().HttpContext.Request.Path;
    //}
    /// <summary>
    /// 获取客户分页列表
    /// </summary>
    /// <param name="input">查询条件</param>
    /// <returns></returns>
    [DataAuthPermission("GetCustomerPageList")]
    public async Task<PagedResultDto<CustomerDto>> GetCustomerPageList(PagedCustomerResultRequestDto input)
    {
        //当前登录用户Id
        var loginUserId = ADTOSharpSession.UserId.Value;
        //如果是管理员，则不校验是否是分享人
        var adminUser = await UserManager.GetAdminAsync();
        // 定义基础查询
        var query = from customer in this._customerRepository.GetAll()
                    join user in this._userRepository.GetAll() on customer.CreatorUserId equals user.Id
                    select new { Customer = customer, User = user };

        #region //如果是管理员，则不校验是否是分享人
        if (loginUserId != adminUser.Id)
        {
            query = query.Where(p => p.Customer.CreatorUserId.Equals(loginUserId)
                || this._shareRepository.GetAll().Any(share => share.CustomerId == p.Customer.Id && share.ToUserId == loginUserId));
        }

        if (!string.IsNullOrWhiteSpace(input.KeyWord))
        {
            query = query.Where(p => p.Customer.Name.Contains(input.KeyWord) || p.Customer.Code == input.KeyWord);
        }

        if (input.DepartmentId != null)
        {
            query = query.Where(p => p.User.DepartmentId == input.DepartmentId);
        }

        if (!string.IsNullOrWhiteSpace(input.CustomerState))
        {
            query = query.Where(p => p.Customer.CustomerState == input.CustomerState);
        }

        if (!string.IsNullOrWhiteSpace(input.CustomerLevel))
        {
            query = query.Where(p => p.Customer.CustomerLevel == input.CustomerLevel);
        }

        if (input.StartDate.HasValue)
        {
            query = query.Where(p => p.Customer.CreationTime >= input.StartDate);
        }

        if (input.EndDate.HasValue)
        {
            query = query.Where(p => p.Customer.CreationTime <= input.EndDate.Value.AddDays(1));
        }

        #region
        //var query1 = this._customerRepository.GetAll()
        //            .Where(c => c.CreatorUserId.Equals(loginUserId) || loginUserId.Equals(from share in this._shareRepository.GetAll().Where(s => s.CustomerId == c.Id).Select(s => s.ToUserId)))
        //            .Join(this._userRepository.GetAll(), customer => customer.CreatorUserId, user => user.Id,
        //            (customer, user) => new { Customer = customer, User = user })
        //            .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Customer.Name.Contains(input.KeyWord) || p.Customer.Code == input.KeyWord)
        //            .WhereIf(input.DepartmentId != null, p => p.User.DepartmentId == input.DepartmentId)
        //            .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerState), p => p.Customer.CustomerState == input.CustomerState)
        //            .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerLevel), p => p.Customer.CustomerLevel == input.CustomerLevel)
        //            .WhereIf(input.StartDate.HasValue, p => p.Customer.CreationTime >= input.StartDate)
        //            .WhereIf(input.EndDate.HasValue, p => p.Customer.CreationTime <= input.EndDate.Value.AddDays(1));
        #endregion

        #endregion

        //数据权限
        //query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, GetRequestPath());
        string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetCustomerPageList)));
        
        query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);
        var totalCount = await query.CountAsync();
        //根据更新日期排序
        var items = await query.OrderByDescending(p => p.Customer.CreationTime).PageBy(input).ToListAsync();
        var list = items.Select(item =>
        {
            //item.User.DepartmentId
            var dto = ObjectMapper.Map<CustomerDto>(item.Customer);
            dto.CustomerLevelName = this._dataItemDetailRepository.GetAll().Where(p => p.ItemValue == item.Customer.CustomerLevel).Select(p => p.ItemName).FirstOrDefault();
            dto.CustomerStateName = this._dataItemDetailRepository.GetAll().Where(p => p.ItemValue == item.Customer.CustomerState).Select(p => p.ItemName).FirstOrDefault();
            //最后跟进时间
            dto.LastFollowTime = this._followRepository.GetAll().Where(p => p.CustomerId == item.Customer.Id).OrderByDescending(p => p.CreationTime).Select(p => (DateTime?)p.CreationTime).FirstOrDefault();
            //联系人电话（第一条联系人）
            dto.FirstContactPhone = this._contactRepository.GetAll().Where(p => p.CustomerId == item.Customer.Id).OrderBy(p => p.CreationTime).Select(p => p.Phone).FirstOrDefault();
            dto.CreatorUserName = item.User?.Name;

            return dto;
        }).ToList();
        return new PagedResultDto<CustomerDto>(totalCount, list);
    }

    /// <summary>
    /// 获取指定客户资料详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<CustomerDetailShowDto> GetCustomerByIdAsync(EntityDto<Guid> input)
    {
        //var info = await _customerRepository.GetAsync(input.Id);
        //var customerDto = ObjectMapper.Map<CustomerDetailShowDto>(info);

        var query = from customer in this._customerRepository.GetAll()
                    where customer.Id == input.Id
                    join d in this._userRepository.GetAll() on customer.ResponsibilityUserId equals d.Id into r_d
                    from user in r_d.DefaultIfEmpty()
                    join u in this._userRepository.GetAll() on customer.CreatorUserId equals u.Id into u_d
                    from creatorUser in u_d.DefaultIfEmpty()
                    select new
                    {
                        customer,
                        username = user.Name,
                        creatorUserName = creatorUser.Name,

                        lastFollow = this._followRepository.GetAll()
                        .Where(f => f.CustomerId == customer.Id)
                        .OrderByDescending(f => f.CreationTime) // 按时间降序
                        .FirstOrDefault()                       // 取最后一条

                    };
        var info = await query.FirstOrDefaultAsync();
        var customerDto = ObjectMapper.Map<CustomerDetailShowDto>(info.customer);
        //归属人
        customerDto.ResponsibilityUserName = info.username;
        //创建人
        customerDto.CreatorUserName = info.creatorUserName;
        customerDto.LastFollowTime = info.lastFollow != null ? info.lastFollow.CreationTime : null;
        //读取客户联系人
        var contactList = await _contactRepository.GetAll().Where(p => p.CustomerId == input.Id).ToListAsync();
        customerDto.CustomerContacts = ObjectMapper.Map<List<CustomerContactsDto>>(contactList);

        //读取客户产品
        var productList = await _productRepository.GetAll().Where(p => p.CustomerId == input.Id).ToListAsync();
        customerDto.CustomerProducts = ObjectMapper.Map<List<CustomerProductDto>>(productList);
        //客户详情附件读取（手机端中有客户新增有附件）
        if (customerDto.FolderId.HasValue && customerDto.FolderId != Guid.Empty)
        {
            var uploadFileList = await this._uploadFileAppService.GetFileUrlListByFolderId(customerDto.FolderId.Value);
            customerDto.UploadFilesDtos = uploadFileList.Select(item =>
            {
                CustomerUploadFilesDto uploadFilesDto = new CustomerUploadFilesDto()
                {
                    FileId = item.Id,
                    Url = item.FullAddress,
                    Name = item.FileName
                };
                return uploadFilesDto;
            }).ToList();
        }

        //最后一级区域名称
        customerDto.LastLocationName= await _locationAppService.GetLastLevelNameAsync(customerDto.AreaId, customerDto.CountryId, customerDto.ProvinceId, customerDto.CityId, customerDto.CountyId);
        return customerDto;
    }

    /// <summary>
    /// 客户审核
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Customers_Audit)]
    public async Task SetCustomerAuditStatus(SetCustomerAuditStatusDto input)
    {
        var info = this._customerRepository.GetAll().Where(p => p.Id == input.CustomerId);
        if (info == null)
        {
            throw new ArgumentNullException("客户信息不存在");
        }
        //指定字段修改
        await this._customerRepository.UpdateAsync(input.CustomerId, async entity =>
        {
            entity.AuditStatus = input.AuditStatus;
            entity.AuditTime = DateTime.Now;
            entity.AuditUserId = ADTOSharpSession.UserId.Value;
        });

        await _logAppService.CreateCustomerLogAsync(new CreateCustomerLogDto() { CustomerId = input.CustomerId, OperateType = "审核", DataDetail = JsonConvert.SerializeObject(input) });
    }

    /// <summary>
    /// 是否存在相同的客户名称、客户邮箱
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<CustomerDto> IsExistCustomerName(CustomerExistInfoDto input)
    {
        // 参数校验
        if (string.IsNullOrWhiteSpace(input.Name) && string.IsNullOrWhiteSpace(input.Email))
        {
            throw new ArgumentException("客户名称和邮箱不能同时为空");
        }

        // 构建查询条件
        var query = _customerRepository.GetAll();

        if (input.Id.HasValue && input.Id.Value != Guid.Empty)
        {
            // 修改场景：排除当前记录
            query = query.Where(p => p.Id != input.Id);
        }

        // 优先检查名称
        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            query = query.Where(p => p.Name == input.Name);
        }
        // 名称不存在时检查邮箱
        else if (!string.IsNullOrWhiteSpace(input.Email))
        {
            query = query.Where(p => p.Email == input.Email);
        }

        var existingCustomer = await query.FirstOrDefaultAsync();

        if (existingCustomer != null)
        {
            throw new UserFriendlyException(
                $"{(!string.IsNullOrWhiteSpace(input.Name) ? "客户名称" : "邮箱")}已存在",
                details: $"重复记录ID: {existingCustomer.Id}");
        }

        return ObjectMapper.Map<CustomerDto>(existingCustomer);
    }

}

