using System;

namespace ADTO.DCloud;

public class AppConsts
{

    public static readonly Guid AdminUserId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid AdminUserId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");



    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 1000;
    public const string DefaultPassPhrase = "f40cc6fff6a34c82af9a03c86a9e6fa9";
    public const int ResizedMaxProfilePictureBytesUserFriendlyValue = 1024;
    public const int MaxProfilePictureBytesUserFriendlyValue = 5;
    public const string TokenValidityKey = "token_validity_key";
    public const string RefreshTokenValidityKey = "refresh_token_validity_key";
    public const string SecurityStampKey = "AspNet.Identity.SecurityStamp";
    public const string TokenType = "token_type";
    public static string UserIdentifier = "user_identifier";
    public const string ThemeDefault = "default";
    public const string Theme2 = "theme2";
    public const string Theme3 = "theme3";
    public const string Theme4 = "theme4";
    public const string Theme5 = "theme5";
    public const string Theme6 = "theme6";
    public const string Theme7 = "theme7";
    public const string Theme8 = "theme8";
    public const string Theme9 = "theme9";
    public const string Theme10 = "theme10";
    public const string Theme11 = "theme11";
    public const string Theme12 = "theme12";
    public const string Theme13 = "theme13";
    public static TimeSpan AccessTokenExpiration = TimeSpan.FromDays(1);
    public static TimeSpan RefreshTokenExpiration = TimeSpan.FromDays(365);
}

