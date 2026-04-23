using System;
using System.Threading.Tasks;
using ADTOSharp.Domain.Services;

namespace ADTO.DCloud.Authorization.Impersonation
{
    public interface IImpersonationManager : IDomainService
    {
        Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken);

        Task<string> GetImpersonationToken(Guid userId, Guid? tenantId);

        Task<string> GetBackToImpersonatorToken();
    }
}