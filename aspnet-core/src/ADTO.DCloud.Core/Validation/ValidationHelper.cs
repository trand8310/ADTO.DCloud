using ADTOSharp.Extensions;
using System.Text.RegularExpressions;

namespace ADTO.DCloud.Validation;

/// <summary>
/// 数据格式校验帮助类
/// </summary>
public static class ValidationHelper
{
    public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    /// <summary>
    /// 检测是否有效邮件件地址
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmail(string value)
    {
        if (value.IsNullOrEmpty())
        {
            return false;
        }

        var regex = new Regex(EmailRegex);
        return regex.IsMatch(value);
    }
}
