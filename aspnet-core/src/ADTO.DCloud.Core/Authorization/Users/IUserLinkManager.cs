using ADTO.DCloud.Authorization.Impersonation;
using ADTOSharp;
using ADTOSharp.Authorization.Users;
using System;
using System.Threading.Tasks;
 
namespace ADTO.DCloud.Authorization.Users
{
    public interface IUserLinkManager
    {
        Task Link(User firstUser, User secondUser);

        Task<bool> AreUsersLinked(UserIdentifier firstUserIdentifier, UserIdentifier secondUserIdentifier);

        Task Unlink(UserIdentifier userIdentifier);

        Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier);

        Task<string> GetAccountSwitchToken(Guid targetUserId, Guid? targetTenantId);

        Task<UserAndIdentity> GetSwitchedUserAndIdentity(string switchAccountToken);
    }
}