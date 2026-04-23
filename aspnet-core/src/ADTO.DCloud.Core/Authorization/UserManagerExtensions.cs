using System.Threading.Tasks;
using ADTOSharp.Authorization.Users;
using ADTO.DCloud.Authorization.Users;

namespace ADTO.DCloud.Authorization;

public static class UserManagerExtensions
{
    /// <summary>
    /// 获取管理员帐号,默认为"admin"
    /// </summary>
    /// <param name="userManager"></param>
    /// <returns></returns>
    public static async Task<User> GetAdminAsync(this UserManager userManager)
    {
        return await userManager.FindByNameAsync(ADTOSharpUserBase.AdminUserName);
    }
    
    public static User GetAdmin(this UserManager userManager)
    {
        return userManager.FindByNameOrEmail(ADTOSharpUserBase.AdminUserName);
    }
}
