using ADTO.DCloud;
using ADTO.DCloud.Web.Authentication.JwtBearer;
using ADTOSharp.Authorization;
using ADTOSharp.Runtime.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using ADTO.DCloud.Configuration;

namespace ADTO.AuthServer.Startup
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {

            var authenticationBuilder = services.AddAuthentication();

            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                authenticationBuilder.AddADTOSharpAsyncJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = configuration["Authentication:JwtBearer:Audience"],

                        // Validate the token expiry
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here
                        ClockSkew = TimeSpan.Zero
                    };
#pragma warning disable CS0618
                    options.SecurityTokenValidators.Clear();
#pragma warning restore CS0618
                    options.AsyncSecurityTokenValidators.Add(new DCloudAsyncJwtSecurityTokenHandler());
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = QueryStringTokenResolver
                    };
                });
            }

        }

        private static Task QueryStringTokenResolver(MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue)
            {
                return Task.CompletedTask;
            }

            if (context.HttpContext.Request.Path.Value.StartsWith("/signalr"))
            {
                var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
                var config = env.GetAppConfiguration();
                var allowAnonymousSignalRConnection = bool.Parse(config["App:AllowAnonymousSignalRConnection"]);

                return SetToken(context, allowAnonymousSignalRConnection);
            }

            List<string> urlsUsingEnchAuthToken = new List<string>()
        {
            "/Chat/GetUploadedObject?",
            "/Profile/GetProfilePictureByUser?"
        };

            if (urlsUsingEnchAuthToken.Any(url => context.HttpContext.Request.GetDisplayUrl().Contains(url)))
            {
                if (context.HttpContext.Request.Headers.ContainsKey("authorization"))
                {
                    return Task.CompletedTask;
                }

                return SetToken(context, false);
            }

            return Task.CompletedTask;
        }

        private static Task SetToken(MessageReceivedContext context, bool allowAnonymous)
        {
            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (qsAuthToken == null)
            {
                if (!allowAnonymous)
                {
                    throw new ADTOSharpAuthorizationException("SignalR auth token is missing.");
                }

                return Task.CompletedTask;
            }
            //string decoded = HttpUtility.UrlDecode(qsAuthToken).Replace(" ", "+");

            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, AppConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}
