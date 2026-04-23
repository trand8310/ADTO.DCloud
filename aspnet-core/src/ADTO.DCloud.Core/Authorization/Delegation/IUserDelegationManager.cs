using System;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Domain.Services;

namespace ADTO.DCloud.Authorization.Delegation
{
    public interface IUserDelegationManager : IDomainService
    {
        Task<bool> HasActiveDelegationAsync(Guid sourceUserId, Guid targetUserId);

        bool HasActiveDelegation(Guid sourceUserId, Guid targetUserId);

        Task RemoveDelegationAsync(Guid userDelegationId, UserIdentifier currentUser);

        Task<UserDelegation> GetAsync(Guid userDelegationId);
    }
}
