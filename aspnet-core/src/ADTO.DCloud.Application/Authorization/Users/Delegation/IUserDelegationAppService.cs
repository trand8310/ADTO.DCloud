using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Authorization.Users.Delegation.Dto;
using System;

namespace ADTO.DCloud.Authorization.Users.Delegation
{
    public interface IUserDelegationAppService : IApplicationService
    {
        Task<PagedResultDto<UserDelegationDto>> GetDelegatedUsers(GetUserDelegationsInput input);

        Task DelegateNewUser(CreateUserDelegationDto input);

        Task RemoveDelegation(EntityDto<Guid> input);

        Task<List<UserDelegationDto>> GetActiveUserDelegations();
    }
}
