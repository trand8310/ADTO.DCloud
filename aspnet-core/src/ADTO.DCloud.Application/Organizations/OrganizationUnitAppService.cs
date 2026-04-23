using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using System.Linq.Dynamic.Core;
using ADTOSharp.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Organizations.Dto;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Modules.Dto;
using ADTOSharp.UI;
using ADTOSharp.Collections.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ADTOSharp.Web.Mvc.Alerts;
using ADTO.DCloud.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using static ADTOSharp.Zero.Configuration.ADTOSharpZeroSettingNames;
using ADTO.DCloud.Authorization.Posts.Dto;
using ADTO.DCloud.Infrastructure;
using static Twilio.TwiML.Voice.Conference;
using Microsoft.AspNetCore.Authorization;
using ADTOSharp.Domain.Entities;




namespace ADTO.DCloud.Organizations;

/// <summary>
/// “组织架构”页面使用的应用程序服务。 
/// </summary>
public class OrganizationUnitAppService : DCloudAppServiceBase, IOrganizationUnitAppService
{
    #region Fields
    private readonly OrganizationUnitManager _organizationUnitManager;
    private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
    private readonly IRepository<UserOrganizationUnit, Guid> _userOrganizationUnitRepository;
    private readonly IRepository<OrganizationUnitRole, Guid> _organizationUnitRoleRepository;
    private readonly IRepository<User, Guid> _useRepository;
    private readonly RoleManager _roleManager;
    #endregion

    #region Ctor
    /// <summary>
    /// 
    /// </summary>
    /// <param name="organizationUnitManager"></param>
    /// <param name="organizationUnitRepository"></param>
    /// <param name="userOrganizationUnitRepository"></param>
    /// <param name="roleManager"></param>
    /// <param name="useRepository"></param>
    /// <param name="organizationUnitRoleRepository"></param>
    public OrganizationUnitAppService(
        OrganizationUnitManager organizationUnitManager,
        IRepository<OrganizationUnit, Guid> organizationUnitRepository,
        IRepository<UserOrganizationUnit, Guid> userOrganizationUnitRepository,
        RoleManager roleManager,
        IRepository<User, Guid> useRepository,
        IRepository<OrganizationUnitRole, Guid> organizationUnitRoleRepository)
    {
        _organizationUnitManager = organizationUnitManager;
        _organizationUnitRepository = organizationUnitRepository;
        _userOrganizationUnitRepository = userOrganizationUnitRepository;
        _useRepository = useRepository;
        _roleManager = roleManager;
        _organizationUnitRoleRepository = organizationUnitRoleRepository;
    }
    #endregion

    #region Utilities
    private async Task<OrganizationUnitDto> CreateOrganizationUnitDtoAsync(OrganizationUnit organizationUnit)
    {
        var dto = ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
        dto.MemberCount =
            await _userOrganizationUnitRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
        return dto;
    }
    #endregion

    #region Methods




    /// <summary>
    /// 公司/部门列表
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync()
    {
        var organizationUnits = await _organizationUnitRepository.GetAllListAsync();

        var organizationUnitMemberCounts = await _userOrganizationUnitRepository.GetAll()
            .GroupBy(x => x.OrganizationUnitId)
            .Select(groupedUsers => new
            {
                organizationUnitId = groupedUsers.Key,
                count = groupedUsers.Count()
            }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

        var organizationUnitRoleCounts = await _organizationUnitRoleRepository.GetAll()
            .GroupBy(x => x.OrganizationUnitId)
            .Select(groupedRoles => new
            {
                organizationUnitId = groupedRoles.Key,
                count = groupedRoles.Count()
            }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

        return new ListResultDto<OrganizationUnitDto>(
            organizationUnits.Select(ou =>
            {
                var organizationUnitDto = ObjectMapper.Map<OrganizationUnitDto>(ou);
                organizationUnitDto.MemberCount = organizationUnitMemberCounts.ContainsKey(ou.Id)
                    ? organizationUnitMemberCounts[ou.Id]
                    : 0;
                organizationUnitDto.RoleCount = organizationUnitRoleCounts.ContainsKey(ou.Id)
                    ? organizationUnitRoleCounts[ou.Id]
                    : 0;
                return organizationUnitDto;
            }).ToList());
    }





    /// <summary>
    /// 获取指定公司/部门的所有用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(GetOrganizationUnitUsersInput input)
    {
        var query = from ouUser in _userOrganizationUnitRepository.GetAll()
                    join ou in _organizationUnitRepository.GetAll() on ouUser.OrganizationUnitId equals ou.Id
                    join user in UserManager.Users on ouUser.UserId equals user.Id
                    where ouUser.OrganizationUnitId == input.Id
                    select new
                    {
                        ouUser,
                        user
                    };

        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

        return new PagedResultDto<OrganizationUnitUserListDto>(
            totalCount,
            items.Select(item =>
            {
                var organizationUnitUserDto = ObjectMapper.Map<OrganizationUnitUserListDto>(item.user);
                organizationUnitUserDto.AddedTime = item.ouUser.CreationTime;
                return organizationUnitUserDto;
            }).ToList());
    }

    /// <summary>
    /// 获取指定公司/部门的所有角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    //public async Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRolesAsync(GetOrganizationUnitRolesInput input)
    //{
    //    var query = from ouRole in _organizationUnitRoleRepository.GetAll()
    //                join ou in _organizationUnitRepository.GetAll() on ouRole.OrganizationUnitId equals ou.Id
    //                join role in _roleManager.Roles on ouRole.RoleId equals role.Id
    //                where ouRole.OrganizationUnitId == input.Id
    //                select new
    //                {
    //                    ouRole,
    //                    role
    //                };

    //    var totalCount = await query.CountAsync();
    //    var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

    //    return new PagedResultDto<OrganizationUnitRoleListDto>(
    //        totalCount,
    //        items.Select(item =>
    //        {
    //            var organizationUnitRoleDto = ObjectMapper.Map<OrganizationUnitRoleListDto>(item.role);
    //            organizationUnitRoleDto.AddedTime = item.ouRole.CreationTime;
    //            return organizationUnitRoleDto;
    //        }).ToList());
    //}

    /// <summary>
    /// 获取指定公司/部门的所有角色,不分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ListResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRolesAsync(GetOrganizationUnitRolesInput input)
    {
        var query = from ouRole in _organizationUnitRoleRepository.GetAll()
                    join ou in _organizationUnitRepository.GetAll() on ouRole.OrganizationUnitId equals ou.Id
                    join role in _roleManager.Roles on ouRole.RoleId equals role.Id
                    where ouRole.OrganizationUnitId == input.Id
                    select new
                    {
                        ouRole,
                        role
                    };

        //var totalCount = await query.CountAsync();
        var items = await query.OrderBy(input.Sorting).ToListAsync();//.PageBy(input)

        return new ListResultDto<OrganizationUnitRoleListDto>(
            //totalCount,
            items.Select(item =>
            {
                var organizationUnitRoleDto = ObjectMapper.Map<OrganizationUnitRoleListDto>(item.role);
                organizationUnitRoleDto.AddedTime = item.ouRole.CreationTime;
                return organizationUnitRoleDto;
            }).ToList());
    }
    /// <summary>
    /// 获取组织架构（依Id）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<OrganizationUnitEditDto> GetOrganizationUnitForEditAsync(EntityDto<Guid> input)
    {
        var entity = await _organizationUnitRepository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<OrganizationUnitEditDto>(entity);
        if (dto.ParentId.HasValue && !dto.ParentId.Value.IsEmpty())
        {
            var parent = await _organizationUnitRepository.GetAsync(dto.ParentId.Value);
            dto.ParentName = parent != null && !string.IsNullOrEmpty(parent.DisplayName) ? parent.DisplayName : "";
        }
        if (dto.ManagerUserId.HasValue)
        {
            var manager = await _useRepository.FirstOrDefaultAsync(dto.ManagerUserId.Value);
            dto.ManagerUserName = manager == null ? "" : manager.Name;
        }
        return dto;
    }

    /// <summary>
    /// 新增公司/部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<OrganizationUnitDto> CreateOrganizationUnitAsync(CreateOrganizationUnitInput input)
    {
        var organizationUnit = new OrganizationUnit(ADTOSharpSession.TenantId, input.DisplayName, input.ParentId);

        await _organizationUnitManager.CreateAsync(organizationUnit);
        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
    }
    /// <summary>
    /// 更新公司/部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<OrganizationUnitDto> UpdateOrganizationUnitAsync(UpdateOrganizationUnitInput input)
    {
        var organizationUnit = await _organizationUnitRepository.GetAsync(input.Id);
        organizationUnit.DisplayName = input.DisplayName;
        organizationUnit.DisplayOrder = input.DisplayOrder;
        organizationUnit.IsActive = input.IsActive;
        organizationUnit.ManagerUserId = input.ManagerUserId;
        organizationUnit.Remark = input.Remark;
        await _organizationUnitManager.UpdateAsync(organizationUnit);

        if ((input.ParentId.HasValue && !organizationUnit.ParentId.HasValue) || (input.ParentId.HasValue && organizationUnit.ParentId.HasValue && input.ParentId.Value != organizationUnit.ParentId.Value))//
        {
            await this.MoveOrganizationUnitAsync(new MoveOrganizationUnitInput() { Id = organizationUnit.Id, NewParentId = input.ParentId });
        }
        return await CreateOrganizationUnitDtoAsync(organizationUnit);
    }

    /// <summary>
    /// 将公司/部门移动到某个上级下面
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<OrganizationUnitDto> MoveOrganizationUnitAsync(MoveOrganizationUnitInput input)
    {
        await _organizationUnitManager.MoveAsync(input.Id, input.NewParentId);

        return await CreateOrganizationUnitDtoAsync(
            await _organizationUnitRepository.GetAsync(input.Id)
        );
    }
    /// <summary>
    /// 删除依据ID指定的公司/部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteOrganizationUnitAsync(EntityDto<Guid> input)
    {
        await _userOrganizationUnitRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
        await _organizationUnitRoleRepository.DeleteAsync(x => x.OrganizationUnitId == input.Id);
        await _organizationUnitManager.DeleteAsync(input.Id);
    }
    /// <summary>
    /// 移除用户与公司/部门的映射关系
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task RemoveUserFromOrganizationUnitAsync(UserToOrganizationUnitInput input)
    {
        await UserManager.RemoveFromOrganizationUnitAsync(input.UserId, input.OrganizationUnitId);
    }
    /// <summary>
    /// 移除公司/部门与角色的映射关系
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task RemoveRoleFromOrganizationUnitAsync(RoleToOrganizationUnitInput input)
    {
        await _roleManager.RemoveFromOrganizationUnitAsync(input.RoleId, input.OrganizationUnitId);
    }
    /// <summary>
    /// 添加用户到公司/部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddUsersToOrganizationUnitAsync(UsersToOrganizationUnitInput input)
    {
        foreach (var userId in input.UserIds)
        {
            await UserManager.AddToOrganizationUnitAsync(userId, input.OrganizationUnitId);
        }
    }
    /// <summary>
    /// 添加角色到公司/部门
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddRolesToOrganizationUnitAsync(RolesToOrganizationUnitInput input)
    {
        foreach (var roleId in input.RoleIds)
        {
            await _roleManager.AddToOrganizationUnitAsync(roleId, input.OrganizationUnitId, ADTOSharpSession.TenantId);
        }
    }
    /// <summary>
    /// 查找公司/部门的用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<NameValueDto>> FindUsersAsync(FindOrganizationUnitUsersInput input)
    {
        var userIdsInOrganizationUnit = _userOrganizationUnitRepository.GetAll()
            .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
            .Select(uou => uou.UserId);

        var query = UserManager.Users
            .Where(u => !userIdsInOrganizationUnit.Contains(u.Id))
            .WhereIf(
                !input.Filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(input.Filter) ||
                    u.UserName.Contains(input.Filter) ||
                    u.EmailAddress.Contains(input.Filter)
            );

        var userCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.Name)
            .PageBy(input)
            .ToListAsync();

        return new PagedResultDto<NameValueDto>(
            userCount,
            users.Select(u =>
                new NameValueDto(
                    u.Name + " (" + u.EmailAddress + ")",
                    u.Id.ToString()
                )
            ).ToList()
        );
    }
    /// <summary>
    /// 查找公司/部门所拥有角色的所有名称
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<NameValueDto>> FindRolesAsync(FindOrganizationUnitRolesInput input)
    {
        var roleIdsInOrganizationUnit = _organizationUnitRoleRepository.GetAll()
            .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
            .Select(uou => uou.RoleId);

        var query = _roleManager.Roles
            .Where(u => !roleIdsInOrganizationUnit.Contains(u.Id))
            .WhereIf(
                !input.Filter.IsNullOrWhiteSpace(),
                u =>
                    u.DisplayName.Contains(input.Filter) ||
                    u.Name.Contains(input.Filter)
            );

        var roleCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.DisplayName)
            .PageBy(input)
            .ToListAsync();

        return new PagedResultDto<NameValueDto>(
            roleCount,
            users.Select(u =>
                new NameValueDto(
                    u.DisplayName,
                    u.Id.ToString()
                )
            ).ToList()
        );
    }


    /// <summary>
    /// 公司/部门列表,一般用于前端做选项用
    /// </summary>
    /// <returns></returns>
    public async Task<List<OrganizationUnitDto>> GetAllAsync()
    {
        var organizationUnits = await _organizationUnitRepository.GetAllListAsync();
        return ObjectMapper.Map<List<OrganizationUnitDto>>(organizationUnits);
    }


    #region Company

    private List<OrganizationUnitNestDto> GenerateOrganizationUnitNestTree(List<OrganizationUnitDto> list, Guid? parentId = null)
    {
        var query = list.Where(w => w.ParentId == parentId);
        return query.Select(item =>
        {
            var model = ObjectMapper.Map<OrganizationUnitNestDto>(item);
            model.ParentId = item.ParentId.HasValue ? item.ParentId : null;
            model.Children = GenerateOrganizationUnitNestTree(list, item.Id);
            return model;
        }).ToList();
    }
    private List<CompanyDto> GenerateCompanyTree(List<OrganizationUnitDto> list, Guid? parentId = null)
    {
        var query = list.Where(w => w.ParentId == parentId);
        return query.Select(item =>
        {
            var model = ObjectMapper.Map<CompanyDto>(item);
            model.ParentId = item.ParentId.HasValue ? item.ParentId : null;
            model.Children = GenerateCompanyTree(list, item.Id);
            return model;
        }).ToList();
    }
    private List<DepartmentDto> GenerateDepartmentTree(List<OrganizationUnitDto> list, OrganizationUnitDto company, Guid? parentId = null, int level = 0)
    {
        var query = list.Where(w => w.Classification > 3 && w.ParentId == parentId);
        return query.Select(item =>
        {
            var model = ObjectMapper.Map<DepartmentDto>(item);

            model.CompanyId = company.Id;
            model.CompanyName = company.DisplayName;
            model.ParentId = level == 0 ? null : item.ParentId;
            model.Children = GenerateDepartmentTree(list, company, item.Id, level + 1);
            return model;
        }).ToList();
    }
    /// <summary>
    /// 组织树
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<OrganizationUnitNestDto>> GetOrganizationUnitTreeAsync(GetOrganizationUnitTreeInput input)
    {
        string code = "";
        Guid? parentId = null;
        if (input.OrganizationId.HasValue)
        {
            var organization = await _organizationUnitRepository.GetAsync(input.OrganizationId.Value);
            code = organization != null && !string.IsNullOrEmpty(organization.Code) ? organization.Code : "";
            parentId = organization != null ? organization.Id : null;
        }
        var query = await _organizationUnitRepository.GetAll().
            WhereIf(!string.IsNullOrEmpty(input.KeyWord), t => t.DisplayName.Contains(input.KeyWord) || t.Code.Contains(input.KeyWord))
            .WhereIf(!string.IsNullOrEmpty(code), t => t.Code.Contains(code))
            .WhereIf(input.IsActive.HasValue, t => t.IsActive.Equals(input.IsActive))
            .ToListAsync();
        var queryList = from or in query
                        join u in _useRepository.GetAll() on or.ManagerUserId equals u.Id into f_u
                        from user in f_u.DefaultIfEmpty()
                        select new { or, user };
        var organizationUnitMemberCounts = await _userOrganizationUnitRepository.GetAll()
           .GroupBy(x => x.OrganizationUnitId)
           .Select(groupedUsers => new
           {
               organizationUnitId = groupedUsers.Key,
               count = groupedUsers.Count()
           }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

        var ou = queryList.Select(item =>
       {
           var dto = ObjectMapper.Map<OrganizationUnitDto>(item.or);
           dto.MemberCount = organizationUnitMemberCounts.ContainsKey(item.or.Id)
                   ? organizationUnitMemberCounts[item.or.Id]
                   : 0;
           if (item.user != null && item.user.Id != Guid.Empty)
           {
               dto.ManagerUser = item.user.Name;
               dto.ManagerUserPhoneNumber = item.user.PhoneNumber;
           }
           return dto;
       }).ToList();
        return new ListResultDto<OrganizationUnitNestDto>(GenerateOrganizationUnitNestTree(ou, parentId));
    }
    /// <summary>
    /// 公司树
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<CompanyDto>> GetCompanyTreeAsync()
    {
        var list = await _organizationUnitRepository.GetAll().Where(w => w.Classification < 4).ToListAsync();
        var companies = ObjectMapper.Map<List<OrganizationUnitDto>>(list);
        return new ListResultDto<CompanyDto>(GenerateCompanyTree(companies, null));
    }

    /// <summary>
    /// 部门树
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<DepartmentDto>> GetDepartmentTreeAsync(EntityDto<Guid?> input)
    {
        var organizations = await _organizationUnitRepository.GetAllListAsync();
        var list = ObjectMapper.Map<List<OrganizationUnitDto>>(organizations);
        var allCompanies = list.Where(w => w.Classification < 4).WhereIf(input.Id.HasValue, w => w.Id == input.Id);
        var allComandyIds = allCompanies.Select(s => s.Id);
        var allDepartments = list.Where(w => allComandyIds.Contains(w.ParentId.Value));
        var result = new List<DepartmentDto>();
        foreach (var company in allCompanies)
        {
            result.AddRange(GenerateDepartmentTree(list, company, company.Id, 0));
        }
        return new ListResultDto<DepartmentDto>(result);

    }


    /// <summary>
    /// 公司列表 cref: /organization/companys
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<OrganizationUnitDto>> GetCompaniesAsync()
    {
        var organizationUnits = await _organizationUnitRepository.GetAll().Where(w => w.Classification < 4).ToListAsync();

        var organizationUnitMemberCounts = await _userOrganizationUnitRepository.GetAll()
            .GroupBy(x => x.OrganizationUnitId)
            .Select(groupedUsers => new
            {
                organizationUnitId = groupedUsers.Key,
                count = groupedUsers.Count()
            }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);

        var organizationUnitRoleCounts = await _organizationUnitRoleRepository.GetAll()
            .GroupBy(x => x.OrganizationUnitId)
            .Select(groupedRoles => new
            {
                organizationUnitId = groupedRoles.Key,
                count = groupedRoles.Count()
            }).ToDictionaryAsync(x => x.organizationUnitId, y => y.count);


        return new ListResultDto<OrganizationUnitDto>(
            organizationUnits.Select(ou =>
            {
                var organizationUnitDto = ObjectMapper.Map<OrganizationUnitDto>(ou);
                organizationUnitDto.MemberCount = organizationUnitMemberCounts.ContainsKey(ou.Id)
                    ? organizationUnitMemberCounts[ou.Id]
                    : 0;
                organizationUnitDto.RoleCount = organizationUnitRoleCounts.ContainsKey(ou.Id)
                    ? organizationUnitRoleCounts[ou.Id]
                    : 0;
                return organizationUnitDto;
            }).ToList());
    }



    private void UpdateDepartmentNest(List<DepartmentJoinedDto> list, DepartmentJoinedDto company, Guid? parentId = null, int level = 0)
    {
        foreach (var item in list.Where(w => w.ParentId == parentId))
        {
            item.CompanyId = company.Id;
            item.CompanyName = company.DisplayName;
            if (list.Any(a => a.ParentId == item.Id))
            {
                UpdateDepartmentNest(list, company, item.Id, level + 1);
            }
        }
    }

    /// <summary>
    /// 部门列表
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<DepartmentInfoDto>> GetDepartmentsAsync(GetDepartmentsInput input)
    {
        var query = from ou in _organizationUnitRepository.GetAll()
                    join user in _useRepository.GetAll() on ou.ManagerUserId equals user.Id into tmp
                    from mu in tmp.DefaultIfEmpty()
                    select new DepartmentJoinedDto() { Id = ou.Id, DisplayName = ou.DisplayName, ParentId = ou.ParentId, Code = ou.Code, Classification = ou.Classification, ManagerUser = mu != null ? ObjectMapper.Map<UserLightDto>(mu) : null };

        var list = await query.ToListAsync();
        var allCompanies = list.Where(w => w.Classification < 4);
        var allComandyIds = allCompanies.Select(s => s.Id);
        var rootDepartments = list.Where(w => w.Classification > 3 && allComandyIds.Contains(w.ParentId.Value));
        var allDepartments = list.Where(w => w.Classification > 3).ToList();

        foreach (var company in allCompanies)
        {
            UpdateDepartmentNest(allDepartments, company, company.Id, 0);
        }

        return new ListResultDto<DepartmentInfoDto>(
            allDepartments.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), w => w.DisplayName.Contains(input.Keyword)).WhereIf(input.CompanyId.HasValue, w => w.CompanyId == input.CompanyId.Value).Select(item =>
            {
                var departmentDto = ObjectMapper.Map<DepartmentInfoDto>(item);
                if (allComandyIds.Any(a => a == departmentDto.ParentId))
                {
                    departmentDto.ParentId = null;
                }
                return departmentDto;
            }).ToList());
    }

    /// <summary>
    /// 部门列表
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ListResultDto<DepartmentFlatDto>> GetDepartmentOfListByIdsAsync(List<Guid> input)
    {
        var list = await _organizationUnitRepository.GetAll().Where(w => input.Contains(w.Id)).ToListAsync();
        return new ListResultDto<DepartmentFlatDto>(ObjectMapper.Map<List<DepartmentFlatDto>>(list));
    }



    #endregion

    /// <summary>
    /// 根据Id获取对应组织架构
    /// </summary>
    /// <returns></returns>

    public async Task<OrganizationUnitDto> GetOrganizationUnitsByIdAsync(EntityDto<Guid> input)
    {
        var entity = await _organizationUnitRepository.GetAsync(input.Id);
        return ObjectMapper.Map<OrganizationUnitDto>(entity);
    }

    /// <summary>
    /// 获取完整部门链名称-organization/department/name/{id}
    /// </summary>
    /// <param name="id">部门ids</param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<string> GetFullName(EntityDto<Guid> input)
    {
        var entity = await _organizationUnitRepository.FirstOrDefaultAsync(t => t.Id.Equals(input.Id));
        var list = await _organizationUnitRepository.GetAll().Where(t => t.IsActive == true && entity.Code.Contains(t.Code)).ToListAsync();
        return GetFullName(entity, list);
    }
    private string GetFullName(OrganizationUnit entity, List<OrganizationUnit> list)
    {
        var res = entity.DisplayName;
        var pEntity = list.Find(t => t.Id == entity.ParentId);
        if (pEntity != null)
        {
            var pRes = GetFullName(pEntity, list);
            res = pRes + "-" + res;
        }
        return res;
    }

    #endregion


    #region 手机端相关接口

    /// <summary>
    /// 手机端中获取组织结构数据
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public async Task<List<OrganizationUnitAppDto>> APP_OrganizationUnitsParentId(Guid? parentId = null)
    {
        var query = this._organizationUnitRepository.GetAll().Where(p => p.IsActive).Where(p => p.ParentId == parentId);
        var list = await query.OrderBy(p => p.DisplayOrder).ToListAsync();

        //所有组织架构，避免多次重复查询
        var allOrgs = await _organizationUnitRepository.GetAll().ToListAsync();
        // 提前查询所有员工信息
        var allEmployees = await this._userOrganizationUnitRepository.GetAll().ToListAsync();

        List<OrganizationUnitAppDto> olist = new List<OrganizationUnitAppDto>();

        foreach (var item in list)
        {
            var itemDto = ObjectMapper.Map<OrganizationUnitAppDto>(item);
            //获取该部门所有子部门（包含本身）
            var organizationIds = GetOrganizationAndChildrenIds(item.Id, allOrgs);

            //是否存在子部门  因为包含了本身，所以需要>1
            itemDto.HasChild = organizationIds.Count > 1;
            itemDto.Employees = allEmployees.Where(p => organizationIds.Contains(p.OrganizationUnitId)).Count();
            //itemDto.Employees = await this._userOrganizationUnitRepository.GetAll().Where(p => organizationIds.Contains(p.OrganizationUnitId)).CountAsync();

            olist.Add(itemDto);
        }
        return olist;
    }

    /// <summary>
    /// 获取指定部门所有子部门ID(包含本身)
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="allOrgs">所有组织架构，避免多次重复查询</param>
    /// <returns></returns>
    private List<Guid> GetOrganizationAndChildrenIds(Guid organizationId, List<OrganizationUnit> allOrgs)
    {
        //var allOrgs = await _organizationUnitRepository.GetAll().ToListAsync();
        var result = new List<Guid> { organizationId };
        AddChildrenIds(organizationId, allOrgs, result);
        return result;
    }

    private void AddChildrenIds(Guid parentId, List<OrganizationUnit> allOrgs, List<Guid> result)
    {
        var children = allOrgs.Where(o => o.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            result.Add(child.Id);
            AddChildrenIds(child.Id, allOrgs, result);
        }
    }

    #endregion
}
