using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync();

        Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(GetOrganizationUnitUsersInput input);

        Task<OrganizationUnitDto> CreateOrganizationUnitAsync(CreateOrganizationUnitInput input);

        Task<OrganizationUnitDto> UpdateOrganizationUnitAsync(UpdateOrganizationUnitInput input);

        Task<OrganizationUnitDto> MoveOrganizationUnitAsync(MoveOrganizationUnitInput input);

        Task DeleteOrganizationUnitAsync(EntityDto<Guid> input);

        Task RemoveUserFromOrganizationUnitAsync(UserToOrganizationUnitInput input);

        Task RemoveRoleFromOrganizationUnitAsync(RoleToOrganizationUnitInput input);

        Task AddUsersToOrganizationUnitAsync(UsersToOrganizationUnitInput input);

        Task AddRolesToOrganizationUnitAsync(RolesToOrganizationUnitInput input);

        Task<PagedResultDto<NameValueDto>> FindUsersAsync(FindOrganizationUnitUsersInput input);

        Task<PagedResultDto<NameValueDto>> FindRolesAsync(FindOrganizationUnitRolesInput input);
        
        Task<List<OrganizationUnitDto>> GetAllAsync();
    }
}
