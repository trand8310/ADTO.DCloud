using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ADTOSharp.Threading;

namespace ADTOSharp.Authorization
{
    public static class AuthorizationHelperExtensions
    {
        public static async Task AuthorizeAsync(this IAuthorizationHelper authorizationHelper, IADTOSharpAuthorizeAttribute authorizeAttribute)
        {
            await authorizationHelper.AuthorizeAsync(new[] { authorizeAttribute });
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper, IADTOSharpAuthorizeAttribute authorizeAttribute)
        {
            authorizationHelper.Authorize(new[] { authorizeAttribute });
        }
    }
}