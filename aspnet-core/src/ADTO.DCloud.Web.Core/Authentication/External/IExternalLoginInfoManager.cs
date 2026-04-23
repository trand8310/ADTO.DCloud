using System.Collections.Generic;
using System.Security.Claims;
using ADTO.DCloud.Authentication.External;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Identity;

namespace ADTO.DCloud.Web.Authentication.External;

public interface IExternalLoginInfoManager : ITransientDependency
{
    string GetUserNameFromClaims(List<Claim> claims);

    string GetUserNameFromExternalAuthUserInfo(ExternalAuthUserInfo userInfo);

    (string name, string surname) GetNameAndSurnameFromClaims(List<Claim> claims, IdentityOptions identityOptions);
}
