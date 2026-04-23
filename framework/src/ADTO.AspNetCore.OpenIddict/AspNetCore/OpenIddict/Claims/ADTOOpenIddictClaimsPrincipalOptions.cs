
using ADTOSharp.Collections;

namespace ADTO.AspNetCore.OpenIddict.Claims;

public class ADTOOpenIddictClaimsPrincipalOptions
{
    public ITypeList<IADTOOpenIddictClaimsPrincipalHandler> ClaimsPrincipalHandlers { get; }

    public ADTOOpenIddictClaimsPrincipalOptions()
    {
        ClaimsPrincipalHandlers = new TypeList<IADTOOpenIddictClaimsPrincipalHandler>();
    }
}