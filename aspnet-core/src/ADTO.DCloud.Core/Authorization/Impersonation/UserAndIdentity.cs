using System.Security.Claims;
using ADTO.DCloud.Authorization.Users;

namespace ADTO.DCloud.Authorization.Impersonation;

public class UserAndIdentity
{
    public User User { get; set; }

    public ClaimsIdentity Identity { get; set; }

    public UserAndIdentity(User user, ClaimsIdentity identity)
    {
        User = user;
        Identity = identity;
    }
}