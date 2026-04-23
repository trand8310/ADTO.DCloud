using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// 数据视图数据处理方法
    /// </summary>
    public class SqlFiltersCommon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SqlFilters(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            //去除执行SQL语句的命令关键字
            source = Regex.Replace(source, "/add", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\+", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\(", "( ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "select", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " insert ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " update ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " delete ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " drop ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " truncate ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " declare ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " xp_cmdshell ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " /add ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " net user ", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, " exec ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " execute ", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, "xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, "0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }

        /// <summary>
        /// 不过滤select关键字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SqlFiltersNotSelect(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            //去除执行SQL语句的命令关键字
            // source = Regex.Replace(source, "select", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "/add", " ", RegexOptions.IgnoreCase);
            // source = Regex.Replace(source, "\\+", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\(", "( ", RegexOptions.IgnoreCase);

            source = Regex.Replace(source, " insert ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " update ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " delete ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " drop ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " truncate ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " declare ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " xp_cmdshell ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " net user ", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, " exec ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " execute ", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, " xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, " 0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }
    }
}
