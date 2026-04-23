using System;
using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace ADTO.DCloud.Authentication.JwtBearer;

public class TokenAuthConfiguration
{
    public SymmetricSecurityKey SecurityKey { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public SigningCredentials SigningCredentials { get; set; }

    /// <summary>
    /// 令牌有效期
    /// </summary>
    public TimeSpan AccessTokenExpiration { get; set; }
    /// <summary>
    /// 刷新令牌有效期
    /// </summary>
    public TimeSpan RefreshTokenExpiration { get; set; }
    /// <summary>
    /// 刷新后令牌有效期
    /// </summary>
    public TimeSpan RefreshedAccessTokenExpiration { get; set; }

    
}
