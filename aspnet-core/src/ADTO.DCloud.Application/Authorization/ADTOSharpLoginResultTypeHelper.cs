using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.UI;
using System;

namespace ADTO.DCloud.Authorization;

public class ADTOSharpLoginResultTypeHelper : ADTOSharpServiceBase, ITransientDependency
{
    public ADTOSharpLoginResultTypeHelper()
    {
        LocalizationSourceName = DCloudConsts.LocalizationSourceName;
    }

    public Exception CreateExceptionForFailedLoginAttempt(ADTOSharpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
    {
        switch (result)
        {
            case ADTOSharpLoginResultType.Success:
                return new Exception("Don't call this method with a success result!");
            case ADTOSharpLoginResultType.InvalidUserNameOrEmailAddress:
            case ADTOSharpLoginResultType.InvalidPassword:
                return new UserFriendlyException(L("LoginFailed"), L("InvalidUserNameOrPassword"));
            case ADTOSharpLoginResultType.InvalidTenancyName:
                return new UserFriendlyException(L("LoginFailed"), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
            case ADTOSharpLoginResultType.TenantIsNotActive:
                return new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", tenancyName));
            case ADTOSharpLoginResultType.UserIsNotActive:
                return new UserFriendlyException(L("LoginFailed"), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
            case ADTOSharpLoginResultType.UserEmailIsNotConfirmed:
                return new UserFriendlyException(L("LoginFailed"), L("UserEmailIsNotConfirmedAndCanNotLogin"));
            case ADTOSharpLoginResultType.LockedOut:
                return new UserFriendlyException(L("LoginFailed"), L("UserLockedOutMessage"));
            default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                Logger.Warn("Unhandled login fail reason: " + result);
                return new UserFriendlyException(L("LoginFailed"));
        }
    }

    public string CreateLocalizedMessageForFailedLoginAttempt(ADTOSharpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
    {
        switch (result)
        {
            case ADTOSharpLoginResultType.Success:
                throw new Exception("Don't call this method with a success result!");
            case ADTOSharpLoginResultType.InvalidUserNameOrEmailAddress:
            case ADTOSharpLoginResultType.InvalidPassword:
                return L("InvalidUserNameOrPassword");
            case ADTOSharpLoginResultType.InvalidTenancyName:
                return L("ThereIsNoTenantDefinedWithName{0}", tenancyName);
            case ADTOSharpLoginResultType.TenantIsNotActive:
                return L("TenantIsNotActive", tenancyName);
            case ADTOSharpLoginResultType.UserIsNotActive:
                return L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress);
            case ADTOSharpLoginResultType.UserEmailIsNotConfirmed:
                return L("UserEmailIsNotConfirmedAndCanNotLogin");
            default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                Logger.Warn("Unhandled login fail reason: " + result);
                return L("LoginFailed");
        }
    }
}

