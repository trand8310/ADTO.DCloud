using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Domain.Repositories;

namespace ADTO.DCloud.Authorization.Users
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        List<Guid> GetPasswordExpiredUserIds(DateTime passwordExpireDate);

        Task<User> FindByPhoneNumberAsync(string phoneNumber);

        void UpdateUsersToChangePasswordOnNextLogin(List<Guid> userIdsToUpdate);
    }
}
