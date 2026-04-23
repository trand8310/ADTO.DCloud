namespace ADTOSharp.Zero.Configuration
{
    public static class ADTOSharpZeroSettingNames
    {
        public static class UserManagement
        {
            /// <summary>
            /// 是否需要邮箱确认才能登录
            /// </summary>
            public const string IsEmailConfirmationRequiredForLogin = "ADTOSharp.Zero.UserManagement.IsEmailConfirmationRequiredForLogin";

            /// <summary>
            /// 帐号锁定设置
            /// </summary>
            public static class UserLockOut
            {
                /// <summary>
                /// "是否开启帐户锁定".
                /// </summary>
                public const string IsEnabled = "ADTOSharp.Zero.UserManagement.UserLockOut.IsEnabled";

                /// <summary>
                /// "操作失败多少次后锁定,默认:5次".
                /// </summary>
                public const string MaxFailedAccessAttemptsBeforeLockout = "ADTOSharp.Zero.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout";

                /// <summary>
                /// "帐户锁定时长,当前时间+锁定秒数,默认10分钟".
                /// </summary>
                public const string DefaultAccountLockoutSeconds = "ADTOSharp.Zero.UserManagement.UserLockOut.DefaultAccountLockoutSeconds";
            }

            public static class TwoFactorLogin
            {
                /// <summary>
                /// "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsEnabled".
                /// </summary>
                public const string IsEnabled = "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsEnabled";

                /// <summary>
                /// "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsEmailProviderEnabled".
                /// </summary>
                public const string IsEmailProviderEnabled = "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsEmailProviderEnabled";

                /// <summary>
                /// "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsSmsProviderEnabled".
                /// </summary>
                public const string IsSmsProviderEnabled = "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsSmsProviderEnabled";

                /// <summary>
                /// "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled".
                /// </summary>
                public const string IsRememberBrowserEnabled = "ADTOSharp.Zero.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled";
            }

            public static class PasswordComplexity
            {
                /// <summary>
                /// 用户密码的最小长度要求
                /// </summary>
                public const string RequiredLength = "ADTOSharp.Zero.UserManagement.PasswordComplexity.RequiredLength";

                /// <summary>
                /// 是否要求密码必须包含“非字母数字字符”（特殊字符）
                /// </summary>
                public const string RequireNonAlphanumeric = "ADTOSharp.Zero.UserManagement.PasswordComplexity.RequireNonAlphanumeric";

                /// <summary>
                /// 是否要求密码中必须包含至少 1 个小写字母（a–z）
                /// </summary>
                public const string RequireLowercase = "ADTOSharp.Zero.UserManagement.PasswordComplexity.RequireLowercase";

                /// <summary>
                /// 是否要求密码中必须包含至少 1 个大写字母（A–Z）
                /// </summary>
                public const string RequireUppercase = "ADTOSharp.Zero.UserManagement.PasswordComplexity.RequireUppercase";

                /// <summary>
                /// 密码中是否必须包含数字（0–9）
                /// </summary>
                public const string RequireDigit = "ADTOSharp.Zero.UserManagement.PasswordComplexity.RequireDigit";
            }
        }

        public static class OrganizationUnits
        {
            /// <summary>
            /// "ADTOSharp.Zero.OrganizationUnits.MaxUserMembershipCount".
            /// </summary>
            public const string MaxUserMembershipCount = "ADTOSharp.Zero.OrganizationUnits.MaxUserMembershipCount";
        }
    }
}