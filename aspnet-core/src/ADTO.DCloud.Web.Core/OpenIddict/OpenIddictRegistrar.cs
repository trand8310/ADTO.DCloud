using ADTO.AspNetCore.OpenIddict.Claims;
using ADTO.OpenIddict.Applications;
using ADTO.OpenIddict.Authorizations;
using ADTO.OpenIddict.Scopes;
using ADTO.OpenIddict.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using Org.BouncyCastle.Tls;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace ADTO.DCloud.Web.OpenIddict
{
    public static class OpenIddictRegistrar
    {
        public static void Register(
            IServiceCollection services,
            IConfigurationRoot configuration,
            Action<OpenIddictCoreOptions> setupOptions)
        {
            services.Configure<ADTOOpenIddictClaimsPrincipalOptions>(options =>
            {
                 options.ClaimsPrincipalHandlers.Add<ADTODefaultOpenIddictClaimsPrincipalHandler>();
            });

            services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(builder =>
                {
 
                    builder
                        .SetDefaultApplicationEntity<OpenIddictApplicationModel>()
                        .SetDefaultAuthorizationEntity<OpenIddictAuthorizationModel>()
                        .SetDefaultScopeEntity<OpenIddictScopeModel>()
                        .SetDefaultTokenEntity<OpenIddictTokenModel>();



                    builder
                     .ReplaceApplicationStore<OpenIddictApplicationModel, ADTOOpenIddictApplicationStore>()
                     .ReplaceAuthorizationStore<OpenIddictAuthorizationModel, ADTOOpenIddictAuthorizationStore>()
                     .ReplaceScopeStore<OpenIddictScopeModel, ADTOOpenIddictScopeStore>()
                     .ReplaceTokenStore<OpenIddictTokenModel, ADTOOpenIddictTokenStore>();


                    builder.ReplaceApplicationManager<OpenIddictApplicationModel, ADTOApplicationManager>();
                    builder.ReplaceAuthorizationManager<OpenIddictAuthorizationModel, ADTOAuthorizationManager>();
                    builder.ReplaceScopeManager<OpenIddictScopeModel, ADTOScopeManager>();
                    builder.ReplaceTokenManager<OpenIddictTokenModel, ADTOTokenManager>();

                    builder.Services.TryAddScoped(provider => (IADTOApplicationManager)provider.GetRequiredService<IOpenIddictApplicationManager>());

                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
 
                    options.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(10));
                    options.SetAccessTokenLifetime(TimeSpan.FromHours(2));
                    options.SetIdentityTokenLifetime(TimeSpan.FromHours(2));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));

                    // Enable the token endpoint.
                    options
                    .SetAuthorizationEndpointUris("connect/authorize", "connect/authorize/callback")
                    // .well-known/oauth-authorization-server
                    // .well-known/openid-configuration
                    //.SetConfigurationEndpointUris()
                    // .well-known/jwks
                    //.SetCryptographyEndpointUris()
                    //.SetDeviceAuthorizationEndpointUris("device")
                    .SetIntrospectionEndpointUris("connect/introspect")
                    .SetEndSessionEndpointUris("connect/endsession")
                    .SetPushedAuthorizationEndpointUris("connect/par")
                    .SetRevocationEndpointUris("connect/revocat")
                    .SetTokenEndpointUris("connect/token")
                    .SetUserInfoEndpointUris("connect/userinfo")
                    .SetEndUserVerificationEndpointUris("connect/verify");


                    //options
                    //    .SetAuthorizationEndpointUris("connect/authorize", "connect/authorize/callback")
                    //    .SetTokenEndpointUris("connect/token")
                    //    .SetUserInfoEndpointUris("connect/userinfo");

                    // Enable the client credentials flow.
                    options
                    .AllowClientCredentialsFlow()
                    .AllowPasswordFlow()
                    .AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow();


                   options.AllowCustomFlow("wechat");

                    var appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                    var certPath = Path.Combine(appData, "openiddict-signing.pfx");
                   
                    // 输出路径到日志文件
                    try
                    {
                        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cert-debug.log");
                        File.WriteAllText(logPath,
                            $"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}{Environment.NewLine}" +
                            $"CertPath: {certPath}{Environment.NewLine}" +
                            $"Exists: {File.Exists(certPath)}{Environment.NewLine}"
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("写日志失败: " + ex);
                    }


                    // OpenIddict 使用


                    var cert = X509CertificateLoader.LoadPkcs12FromFile(
                     certPath,
                     "123456".AsSpan(),
                     X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet
                 );




                    // 注册同一张证书为 Signing Key
                    options.AddSigningKey(new X509SecurityKey(cert));

                    // 同一张证书也注册为 Encryption Certificate
                    options.AddEncryptionCertificate(cert);



                    //options.AddProductionEncryptionAndSigningCertificate(certPath, "123456");
                    // Register the signing and encryption credentials.
                    //options.AddDevelopmentEncryptionCertificate()
                    //    .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough()
                        //.EnableLogoutEndpointPassthrough()
                        .EnableEndUserVerificationEndpointPassthrough()
                        .EnableStatusCodePagesIntegration()
                        .DisableTransportSecurityRequirement();


                    options.DisableAccessTokenEncryption();
                })

                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    options.AddAudiences("DCloud");
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();
                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            //services.AddHostedService<OpenIdDictDataSeedWorker>();
        }

        public static void Register(IServiceCollection services, IConfigurationRoot configuration)
        {
            Register(services, configuration, options => { });
        }
    }
}
