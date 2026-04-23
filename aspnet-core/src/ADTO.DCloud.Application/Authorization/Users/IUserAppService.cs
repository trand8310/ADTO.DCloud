using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;
using System;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Roles.Dto;
using ADTO.DCloud.Dto;
using System.Collections.Generic;

namespace ADTO.DCloud.Authorization.Users;

public interface IUserAppService : IApplicationService
{
    Task<PagedResultDto<UserListDto>> GetUsersAsync(GetUsersInput input);
    Task<UserDto> GetUserByIdAsync(EntityDto<Guid> input);

    Task<FileDto> GetUsersToExcelAsync(GetUsersToExcelInput input);

    Task<List<string>> GetUserExcelColumnsToExcelAsync();

    Task<GetUserForEditOutput> GetUserForEditAsync(NullableIdDto<Guid> input);

    Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEditAsync(EntityDto<Guid> input);

    Task ResetUserSpecificPermissionsAsync(EntityDto<Guid> input);

    Task UpdateUserPermissionsAsync(UpdateUserPermissionsInput input);

    Task<User> CreateOrUpdateUserAsync(CreateOrUpdateUserInput input);

    Task DeleteUserAsync(EntityDto<Guid> input);

}
