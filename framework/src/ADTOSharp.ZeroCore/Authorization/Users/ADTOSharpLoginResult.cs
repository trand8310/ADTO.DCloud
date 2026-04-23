using System;
using System.Security.Claims;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Authorization.Users;

public class ADTOSharpLoginResult<TTenant, TUser>
    where TTenant : ADTOSharpTenant<TUser>
    where TUser : ADTOSharpUserBase
{
    public ADTOSharpLoginResultType Result { get; private set; }

    public ILocalizableString FailReason { get; private set; }

    public TTenant Tenant { get; private set; }

    public TUser User { get; private set; }

    public ClaimsIdentity Identity { get; private set; }

    public ADTOSharpLoginResult(ADTOSharpLoginResultType result, TTenant tenant = null, TUser user = null)
    {
        Result = result;
        Tenant = tenant;
        User = user;
    }

    public ADTOSharpLoginResult(TTenant tenant, TUser user, ClaimsIdentity identity)
        : this(ADTOSharpLoginResultType.Success, tenant)
    {
        User = user;
        Identity = identity;
    }

    /// <summary>
    /// This method can be used only when <see cref="Result"/> is <see cref="ADTOSharpLoginResultType.FailedForOtherReason"/>.
    /// </summary>
    /// <param name="failReason">Localizable fail reason message</param>
    public void SetFailReason(ILocalizableString failReason)
    {
        if (Result != ADTOSharpLoginResultType.FailedForOtherReason)
        {
            throw new Exception($"Can not set fail reason for result type {Result}, use this method only for ADTOSharpLoginResultType.FailedForOtherReason result type!");
        }

        FailReason = failReason;
    }

    public string GetFailReason(ILocalizationContext localizationContext)
    {
        return FailReason == null ? string.Empty : FailReason?.Localize(localizationContext);
    }
}