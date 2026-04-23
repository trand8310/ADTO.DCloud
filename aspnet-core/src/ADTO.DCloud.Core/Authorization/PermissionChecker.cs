using ADTOSharp.Authorization;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;

namespace ADTO.DCloud.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
