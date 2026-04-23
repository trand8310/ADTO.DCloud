using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ADTOSharp.Extensions;

namespace ADTO.DCloud.Identity
{
    public class ExternalLoginInfoHelper
    {
        public static string  GetNameFromClaims(List<Claim> claims)
        {
            string name = null;

            var givennameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givennameClaim != null && !givennameClaim.Value.IsNullOrEmpty())
            {
                name = givennameClaim.Value;
            }

            if (name == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var username = nameClaim.Value;
                    if (!username.IsNullOrEmpty())
                    {
                        name = username;
                    }
                }
            }

            return name;
        }
    }
}
