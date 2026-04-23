namespace ADTO.DCloud.Security;

/// <summary>
/// 密码复杂度设定
/// </summary>
public class PasswordComplexitySetting
{
    public int AllowedMinimumLength { get; private set; } = 3;
    public bool Equals(PasswordComplexitySetting other)
    {
        if (other == null)
        {
            return false;
        }

        return
            RequireDigit == other.RequireDigit &&
            RequireLowercase == other.RequireLowercase &&
            RequireNonAlphanumeric == other.RequireNonAlphanumeric &&
            RequireUppercase == other.RequireUppercase &&
            RequiredLength == other.RequiredLength;
    }

    /// <summary>
    /// 必须存在数字
    /// </summary>
    public bool RequireDigit { get; set; }

    /// <summary>
    /// 必须存在小写
    /// </summary>
    public bool RequireLowercase { get; set; }
    /// <summary>
    /// 必须存在非字母数字类字符
    /// </summary>

    public bool RequireNonAlphanumeric { get; set; }
    /// <summary>
    /// 必须存在大写
    /// </summary>
    public bool RequireUppercase { get; set; }
    /// <summary>
    /// 密码的最小长度
    /// </summary>
    public int RequiredLength { get; set; }
}
