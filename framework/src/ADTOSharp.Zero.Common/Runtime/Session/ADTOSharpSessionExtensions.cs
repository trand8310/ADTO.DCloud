using System;
using ADTOSharp.Authorization.Users;

namespace ADTOSharp.Runtime.Session
{
    public static class ADTOSharpSessionExtensions
    {
        public static bool IsUser(this IADTOSharpSession session, ADTOSharpUserBase user)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return session.TenantId == user.TenantId && 
                session.UserId.HasValue && 
                session.UserId.Value == user.Id;
        }
    }
}
