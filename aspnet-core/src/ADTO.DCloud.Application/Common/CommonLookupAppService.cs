using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Users.Profile;
using ADTO.DCloud.Common.Dto;
using ADTO.DCloud.Customers.CustomerFollowManage.Dto;
using ADTO.DCloud.Editions;
using ADTO.DCloud.Editions.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.ProjectManage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;



namespace ADTO.DCloud.Common;
/// <summary>
/// 通用用户查找服务,用于系统中查找用户数据时为列表提供数据
/// </summary>
[ADTOSharpAuthorize]
public class CommonLookupAppService : DCloudAppServiceBase, ICommonLookupAppService
{
    #region Fields
    private readonly EditionManager _editionManager;
    private readonly IProfileAppService _profileAppService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IRepository<EmployeeInfo, Guid> _employeeRepository;
    #endregion


    #region Ctor

    public CommonLookupAppService(EditionManager editionManager, IProfileAppService profileAppService, IWebHostEnvironment webHostEnvironment,
        IRepository<EmployeeInfo, Guid> employeeRepository)
    {
        _editionManager = editionManager;
        _profileAppService = profileAppService;
        _webHostEnvironment = webHostEnvironment;
        _employeeRepository = employeeRepository;

    }
    #endregion

    #region Methods

    /// <summary>
    /// 系统版本号列表
    /// </summary>
    /// <param name="onlyFreeItems"></param>
    /// <returns></returns>
    [HiddenApi]
    public async Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false)
    {
        var subscribableEditions = (await _editionManager.Editions.Cast<SubscribableEdition>().ToListAsync())
            .WhereIf(onlyFreeItems, e => e.IsFree)
            .OrderBy(e => e.MonthlyPrice);

        return new ListResultDto<SubscribableEditionComboboxItemDto>(
            subscribableEditions.Select(e => new SubscribableEditionComboboxItemDto(e.Id.ToString(), e.DisplayName, e.IsFree)).ToList()
        );
    }
    /// <summary>
    /// 查询员工信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<FindUsersOutputDto>> FindEmployees(FindUsersInput input)
    {
        if (ADTOSharpSession.TenantId != null)
        {
            input.TenantId = ADTOSharpSession.TenantId;
        }
        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {
            var query = _employeeRepository.GetAllIncluding(d => d.Department, c => c.Company, u => u.User).WhereIf(
                     !input.Keyword.IsNullOrWhiteSpace(),
                     u =>
                         u.Name.Contains(input.Keyword) ||
                         u.UserName.Contains(input.Keyword) ||
                         u.Email.Contains(input.Keyword)
                 )
                 .WhereIf(input.DepartmentId.HasValue, u => u.Department.Id == input.DepartmentId)
                .WhereIf(input.ExcludeCurrentUser, u => u.Id != ADTOSharpSession.GetUserId());


            var userCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreationTime)
                .PageBy(input)
                .ToListAsync();
            var userDtos = users.Select(item =>
            {
                var dto = ObjectMapper.Map<FindUsersOutputDto>(item.User);
                return dto;
            }).ToList();
            foreach (var item in userDtos)
            {
                var imageUser = await _profileAppService.GetProfilePictureByUser(item.Id);
                //默认图像
                if (string.IsNullOrWhiteSpace(imageUser.ProfilePicture))
                {
                    byte[] defaultImageBytes = File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, "Common", "Images", "default-profile-picture.png"));
                    item.UserProfilePicture = Convert.ToBase64String(defaultImageBytes); // string
                }
                else
                {
                    item.UserProfilePicture = imageUser.ProfilePicture;
                }
            }
            return new PagedResultDto<FindUsersOutputDto>(userCount, userDtos);
        }

    }

    /// <summary>
    /// 查询员工信息(多部门查询)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<FindUsersOutputDto>> FindEmployeesByDepartmentIds(FindUsersInput input)
    {
        if (ADTOSharpSession.TenantId != null)
        {
            input.TenantId = ADTOSharpSession.TenantId;
        }
        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {
            var query = _employeeRepository.GetAllIncluding(d => d.Department, c => c.Company, u => u.User).WhereIf(
                     !input.Keyword.IsNullOrWhiteSpace(),
                     u =>
                         u.Name.Contains(input.Keyword) ||
                         u.UserName.Contains(input.Keyword) ||
                         u.Email.Contains(input.Keyword)
                 )
                .WhereIf(input.DepartmentIds.Count > 0, u => input.DepartmentIds.Contains(u.Department.Id))
                .WhereIf(input.ExcludeCurrentUser, u => u.Id != ADTOSharpSession.GetUserId());


            var userCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreationTime)
                .PageBy(input)
                .ToListAsync();
            var userDtos = users.Select(item =>
            {
                var dto = ObjectMapper.Map<FindUsersOutputDto>(item.User);
                return dto;
            }).ToList();
            foreach (var item in userDtos)
            {
                var imageUser = await _profileAppService.GetProfilePictureByUser(item.Id);
                //默认图像
                if (string.IsNullOrWhiteSpace(imageUser.ProfilePicture))
                {
                    byte[] defaultImageBytes = File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, "Common", "Images", "default-profile-picture.png"));
                    item.UserProfilePicture = Convert.ToBase64String(defaultImageBytes); // string
                }
                else
                {
                    item.UserProfilePicture = imageUser.ProfilePicture;
                }
            }
            return new PagedResultDto<FindUsersOutputDto>(userCount, userDtos);
        }

    }


    /// <summary>
    /// 查找用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
        //[ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users)]
    public async Task<PagedResultDto<FindUsersOutputDto>> FindUsers(FindUsersInput input)
    {
        if (ADTOSharpSession.TenantId != null)
        {
            input.TenantId = ADTOSharpSession.TenantId;
        }

        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {
            var query = UserManager.Users.Include(o => o.Department).Include(x => x.Company)
                .WhereIf(
                    !input.Keyword.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Keyword) ||
                        u.UserName.Contains(input.Keyword) ||
                        u.EmailAddress.Contains(input.Keyword)
                )
                .WhereIf(input.DepartmentId.HasValue, u => u.DepartmentId.Equals(input.DepartmentId))
                .WhereIf(input.CompanyId.HasValue, u => u.CompanyId.Equals(input.CompanyId))
                .WhereIf(input.ExcludeCurrentUser, u => u.Id != ADTOSharpSession.GetUserId())
                .WhereIf(input.Ids != null && input.Ids.Count > 0, q => input.Ids.Contains(q.Id));



            var userCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreationTime)
                .PageBy(input)
                .ToListAsync();

            var userDtos = ObjectMapper.Map<List<FindUsersOutputDto>>(users);
            foreach (var item in userDtos)
            {
                var imageUser = await _profileAppService.GetProfilePictureByUser(item.Id);
                //默认图像
                if (string.IsNullOrWhiteSpace(imageUser.ProfilePicture))
                {
                    byte[] defaultImageBytes = File.ReadAllBytes(Path.Combine(_webHostEnvironment.WebRootPath, "Common", "Images", "default-profile-picture.png"));
                    item.UserProfilePicture = Convert.ToBase64String(defaultImageBytes); // string
                }
                else
                {
                    item.UserProfilePicture = imageUser.ProfilePicture;
                }
            }

            return new PagedResultDto<FindUsersOutputDto>(userCount, userDtos);
        }
    }
    /// <summary>
    /// 用户列表(根据用户主键集合)
    /// </summary>
    /// <param name="keyValues">用户主键集合主键</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IEnumerable<UserDto>> GetListByKeyValues(List<Guid> keyValues)
    {
        var query = await UserManager.Users.Where(u => keyValues.Contains(u.Id) && u.IsActive == true).ToListAsync();
        return ObjectMapper.Map<List<UserDto>>(query);
    }
    /// <summary>
    /// 获取当前系统的版本
    /// </summary>
    /// <returns></returns>
    public GetDefaultEditionNameOutput GetDefaultEditionName()
    {
        return new GetDefaultEditionNameOutput
        {
            Name = EditionManager.DefaultEditionName
        };
    }
    #endregion
}

