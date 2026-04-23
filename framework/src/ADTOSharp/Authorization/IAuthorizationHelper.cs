using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ADTOSharp.Authorization
{
    public interface IAuthorizationHelper
    {
        Task AuthorizeAsync(IEnumerable<IADTOSharpAuthorizeAttribute> authorizeAttributes);
        
        void Authorize(IEnumerable<IADTOSharpAuthorizeAttribute> authorizeAttributes);

        Task AuthorizeAsync(MethodInfo methodInfo, Type type);
        
        void Authorize(MethodInfo methodInfo, Type type);
    }
}