using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ADTO.DCloud.Identity;

/// <summary>
/// 授权认证扩展
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// 返回一个新的IEnumerable来替换基于newClaim 的claim列表。
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <param name="newClaim"></param>
    /// <returns></returns>
    public static IEnumerable<Claim> ReplaceClaim(this IEnumerable<Claim> claimsIdentity, Claim newClaim)
    {
        return claimsIdentity.Select(claim => claim.Type == newClaim.Type ? newClaim : claim);
    }
    /// <summary>
    /// 移除当前用户的声明,再用新的声明来替换,一般用于新用户登录时,将原用户声明过期
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <param name="newClaim"></param>
    public static void ReplaceClaim(this ClaimsIdentity claimsIdentity, Claim newClaim)
    {
        var claim = claimsIdentity.FindFirst(newClaim.Type);
        if (claim != null)
        {
            claimsIdentity.RemoveClaim(claim);
        }

        claimsIdentity.AddClaim(newClaim);
    }
}