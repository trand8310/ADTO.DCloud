namespace ADTOSharp.Authorization
{
    public enum ADTOSharpLoginResultType : byte
    {
        Success = 1,

        InvalidUserNameOrEmailAddress,
        
        InvalidPassword,
        
        UserIsNotActive,

        InvalidTenancyName,
        
        TenantIsNotActive,

        UserEmailIsNotConfirmed,
        
        UnknownExternalLogin,

        LockedOut,

        UserPhoneNumberIsNotConfirmed,
        
        FailedForOtherReason
    }
}