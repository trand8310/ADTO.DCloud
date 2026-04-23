using Microsoft.AspNetCore.Http;
using System;

namespace ADTOSharp.Web.Security.AntiForgery
{
    public class ADTOSharpAntiForgeryConfiguration : IADTOSharpAntiForgeryConfiguration
    {
        public CookieBuilder TokenCookie { get; }
        public string TokenCookieName { get; set; }

        public string TokenHeaderName { get; set; }

        public string AuthorizationCookieName { get; set; }

        public string AuthorizationCookieApplicationScheme { get; set; }
        
        public ADTOSharpAntiForgeryConfiguration()
        {
            TokenCookieName = "XSRF-TOKEN";
            TokenHeaderName = "X-XSRF-TOKEN";
            AuthorizationCookieName = ".AspNet.ApplicationCookie";
            AuthorizationCookieApplicationScheme = "Identity.Application";

            TokenCookie = new CookieBuilder
            {
                Name = TokenCookieName,
                HttpOnly = false,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Expiration = TimeSpan.FromDays(3650) //10 years!
            };
        }
    }
}