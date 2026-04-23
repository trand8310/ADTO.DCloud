using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore.Repositories;
using ADTOSharp.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Z.EntityFramework.Plus;



namespace ADTO.DCloud.Authorization.Users;

public class UserRepository : DCloudRepositoryBase<User, Guid>, IUserRepository
{
    public UserRepository(IDbContextProvider<DCloudDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    ///获取所有密码已经过期的用户IDs
    /// </summary>
    /// <param name="passwordExpireDate"></param>
    /// <returns></returns>
    public List<Guid> GetPasswordExpiredUserIds(DateTime passwordExpireDate)
    {
        var context = GetContext();
        return (
            from user in GetAll()
            let lastRecentPasswordOfUser = context.RecentPasswords
                .Where(rp => rp.UserId == user.Id && rp.TenantId == user.TenantId)
                .OrderByDescending(rp => rp.CreationTime).FirstOrDefault()
            where user.IsActive && !user.ShouldChangePasswordOnNextLogin &&
                  (
                      (lastRecentPasswordOfUser != null &&
                       lastRecentPasswordOfUser.CreationTime <= passwordExpireDate) ||
                      (lastRecentPasswordOfUser == null && user.CreationTime <= passwordExpireDate)
                  )
            select user.Id
        ).Distinct().ToList();

    }

    /// <summary>
    /// 批量更改所有下次登录需要修改密码的IDs
    /// </summary>
    /// <param name="userIdsToUpdate"></param>
    public void UpdateUsersToChangePasswordOnNextLogin(List<Guid> userIdsToUpdate)
    {
        //    GetAll()
        //        .Where(user =>
        //            user.IsActive &&
        //            !user.ShouldChangePasswordOnNextLogin &&
        //            userIdsToUpdate.Contains(user.Id)
        //        )
        //        .Update(x => new User { ShouldChangePasswordOnNextLogin = true });
    }
    /// <summary>
    /// 获取用户的电话号码
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<User> FindByPhoneNumberAsync(string phoneNumber)
    {
        return await FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber);
    }
}

