using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.Timing;

namespace ADTO.DCloud.Authorization.Users;

/// <summary>
/// 用户
/// </summary>
[Description("用户")]
public class User : ADTOSharpUser<User>
{
    public const string DefaultPassword = "123qwe";

    /// <summary>
    /// 邮箱地址
    /// </summary>
    [Description("邮箱地址"), Required(AllowEmptyStrings = true)]
    public override string EmailAddress { get; set; }

    /// <summary>
    /// 邮箱地址,大写
    /// </summary>
    [Description("邮箱地址"), Required(AllowEmptyStrings = true)]
    public override string NormalizedEmailAddress { get; set; }
    /// <summary>
    /// 用户图像ID
    /// </summary>
    [Description("图像ID")]
    public virtual Guid? ProfilePictureId { get; set; }
    /// <summary>
    /// 用户所属的组织单元,一个用户可以属于多个部门,其中有一个主要的部门,该值同时会写入DepartmentId
    /// </summary>
    [Description("组织列表")]
    public List<UserOrganizationUnit> OrganizationUnits { get; set; }

    /// <summary>
    /// 直属上级
    /// </summary>
    public Guid? ManagerId { get; set; }
    /// <summary>
    /// 用户默认部门
    /// </summary>
    [ForeignKey("DepartmentId")]
    public virtual Guid? DepartmentId { get; set; }
    /// <summary>
    /// 用户默认部门
    /// </summary>
    public virtual OrganizationUnit? Department { get; set; }
    /// <summary>
    /// 用户所属的公司
    /// </summary>
    [ForeignKey("CompanyId")]
    public virtual Guid? CompanyId { get; set; }
    /// <summary>
    /// 用户所属的公司,一个用户假设只会存在于一个公司主体
    /// </summary>
    public virtual OrganizationUnit? Company { get; set; }
    /// <summary>
    /// 用户下次登录使用时,必须更改密码
    /// </summary>
    public virtual bool ShouldChangePasswordOnNextLogin { get; set; }
    /// <summary>
    /// 登录TOKEN失效时间
    /// </summary>
    public DateTime? SignInTokenExpireTimeUtc { get; set; }
    /// <summary>
    /// 用户登录时的TOKEN
    /// </summary>
    public string SignInToken { get; set; }
    /// <summary>
    /// 社交登录授权KEY,目前还未使用,可以忽略
    /// </summary>
    public string GoogleAuthenticatorKey { get; set; }
    /// <summary>
    /// 恢复代码,用于恢复密码,
    /// </summary>
    [Description("恢复代码")]
    public string RecoveryCode { get; set; }
    /// <summary>
    /// 性别
    /// </summary>
    [Description("性别")]
    public string Gender { get; set; }

    public static string CreateRandomPassword()
    {
        return Guid.NewGuid().ToString("N").Truncate(16);
    }

    public static User CreateTenantAdminUser(Guid tenantId, string emailAddress, string name = null)
    {
        var user = new User
        {
            TenantId = tenantId,
            UserName = AdminUserName,
            Name = string.IsNullOrWhiteSpace(name) ? AdminUserName : name,
            EmailAddress = emailAddress,
            Roles = new List<UserRole>(),
            OrganizationUnits = new List<UserOrganizationUnit>(),
        };

        user.SetNormalizedNames();

        return user;
    }

    public override void SetNormalizedNames()
    {
        NormalizedUserName = UserName.ToUpperInvariant();
        NormalizedEmailAddress = EmailAddress?.ToUpperInvariant();
    }


    public override void SetNewPasswordResetCode()
    {
        /* 重置代码在URL中使用
         */
        PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
    }

    public void Unlock()
    {
        AccessFailedCount = 0;
        LockoutEndDateUtc = null;
    }

    public void SetSignInToken()
    {
        SignInToken = Guid.NewGuid().ToString();
        SignInTokenExpireTimeUtc = Clock.Now.AddMinutes(1).ToUniversalTime();
    }
}
