namespace ADTOSharp.Net.Mail
{
    /// <summary>
    /// Declares names of the settings defined by <see cref="EmailSettingProvider"/>.
    /// </summary>
    public static class EmailSettingNames
    {
        /// <summary>
        /// ADTOSharp.Net.Mail.DefaultFromAddress
        /// </summary>
        public const string DefaultFromAddress = "ADTOSharp.Net.Mail.DefaultFromAddress";

        /// <summary>
        /// ADTOSharp.Net.Mail.DefaultFromDisplayName
        /// </summary>
        public const string DefaultFromDisplayName = "ADTOSharp.Net.Mail.DefaultFromDisplayName";

        /// <summary>
        /// SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.Host
            /// </summary>
            public const string Host = "ADTOSharp.Net.Mail.Smtp.Host";

            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.Port
            /// </summary>
            public const string Port = "ADTOSharp.Net.Mail.Smtp.Port";

            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.UserName
            /// </summary>
            public const string UserName = "ADTOSharp.Net.Mail.Smtp.UserName";

            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.Password
            /// </summary>
            public const string Password = "ADTOSharp.Net.Mail.Smtp.Password";

            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.Domain
            /// </summary>
            public const string Domain = "ADTOSharp.Net.Mail.Smtp.Domain";

            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.EnableSsl
            /// </summary>
            public const string EnableSsl = "ADTOSharp.Net.Mail.Smtp.EnableSsl";

            /// <summary>
            /// ADTOSharp.Net.Mail.Smtp.UseDefaultCredentials
            /// </summary>
            public const string UseDefaultCredentials = "ADTOSharp.Net.Mail.Smtp.UseDefaultCredentials";
        }
    }
}