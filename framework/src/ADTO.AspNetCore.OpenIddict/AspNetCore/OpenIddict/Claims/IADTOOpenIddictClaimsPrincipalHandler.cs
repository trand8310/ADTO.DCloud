using System.Threading.Tasks;

namespace ADTO.AspNetCore.OpenIddict.Claims;

public interface IADTOOpenIddictClaimsPrincipalHandler
{
    Task HandleAsync(ADTOOpenIddictClaimsPrincipalHandlerContext context);
}