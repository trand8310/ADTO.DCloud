using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;
using System;
using ADTO.DCloud.Authorization.Roles.Dto;

namespace ADTO.DCloud.Authorization.Roles;

public interface IRoleAppService : IApplicationService
{
    /// <summary>
    /// 角色列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input);
    /// <summary>
    /// 依ID获取角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<RoleDto> GetRoleByIdAsync(EntityDto<Guid> input);
    /// <summary>
    /// 获取一个用于编辑时使用的角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<GetRoleForEditOutput> GetRoleForEditAsync(NullableIdDto<Guid> input);
    /// <summary>
    /// 新增/编辑角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task CreateOrUpdateRoleAsync(CreateOrUpdateRoleInput input);
    /// <summary>
    /// 依ID删除角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task DeleteRoleAsync(EntityDto<Guid> input);
}
