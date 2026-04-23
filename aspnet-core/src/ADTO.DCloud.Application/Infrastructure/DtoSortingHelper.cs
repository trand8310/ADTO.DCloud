using System;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure;

public static class DtoSortingHelper
{
    public static string ReplaceSorting(string sorting, Func<string, string> replaceFunc)
    {
        var sortFields = sorting.Split(',');
        for (var i = 0; i < sortFields.Length; i++)
        {
            sortFields[i] = replaceFunc(sortFields[i].Trim());
        }

        return string.Join(",", sortFields);
    }
    //
    // 摘要:
    //     获取指定长度的数字字串
    //
    // 参数:
    //   num:
    //
    //   len:
    public static string NumToStr(int num, int len)
    {
        string text = num.ToString();
        if (text.Length < len)
        {
            int num2 = len - text.Length;
            for (int i = 0; i < num2; i++)
            {
                text = "0" + text;
            }
        }

        return text;
    }
    //
    // 摘要:
    //     将值的首字母大写
    //
    // 参数:
    //   value:
    //     值
    public static string FirstUpper(string value)
    {
        string text = value.Substring(0, 1).ToUpper();
        return text + value.Substring(1, value.Length - 1);
    }

    //
    // 摘要:
    //     将值的首字母小写
    //
    // 参数:
    //   value:
    //     值
    public static string FirstLower(string value)
    {
        string text = value.Substring(0, 1).ToLower();
        return text + value.Substring(1, value.Length - 1);
    }
    /// <summary>
    /// 获取随机数
    /// </summary>
    /// <param name="typevalue"></param>
    /// <returns></returns>
    public static string GetRndStrOrNum(string typevalue)
    {
        var rndstrornumstr = string.Empty;
        switch (typevalue)
        {
            case "num4"://4位随机数
                rndstrornumstr = CommonHelper.RndNum(4).ToString();
                break;
            case "num6"://6位随机数
                rndstrornumstr = CommonHelper.RndNum(6).ToString();
                break;
            case "num8"://8位随机数
                rndstrornumstr = CommonHelper.RndNum(8).ToString();
                break;
            case "numletter4"://4位随机数(数字+字母)
                rndstrornumstr = GetNumLetterStr(4);
                break;
            case "numletter6"://6位随机数(数字+字母)
                rndstrornumstr = GetNumLetterStr(6);
                break;
            case "numletter8"://8位随机数(数字+字母)
                rndstrornumstr = GetNumLetterStr(8);
                break;
            default:
                break;
        }
        return rndstrornumstr;
    }
    /// <summary>
    /// 获取数字字母混合字符串
    /// </summary>
    /// <param name="codeCount"></param>
    /// <returns></returns>
    public static string GetNumLetterStr(int codeCount)
    {
        int rep = 0;
        string str = string.Empty;
        long num2 = DateTime.Now.Ticks + rep;
        rep++;
        Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
        for (int i = 0; i < codeCount; i++)
        {
            char ch;
            int num = random.Next();
            if ((num % 2) == 0)
            {
                ch = (char)(0x30 + ((ushort)(num % 10)));
            }
            else
            {
                ch = (char)(0x41 + ((ushort)(num % 0x1a)));
            }
            str = str + ch.ToString();
        }
        return str;

    }
}

