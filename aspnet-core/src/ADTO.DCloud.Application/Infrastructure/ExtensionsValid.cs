using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    public static class ExtensionsValid
    {
        //
        // 摘要:
        //     检测空值,为null则抛出ArgumentNullException异常
        //
        // 参数:
        //   obj:
        //     对象
        //
        //   parameterName:
        //     参数名
        public static void CheckNull(this object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        //
        // 摘要:
        //     是否为空
        //
        // 参数:
        //   value:
        //     值
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        //
        // 摘要:
        //     是否为空
        //
        // 参数:
        //   value:
        //     值
        public static bool IsEmpty(this Guid? value)
        {
            if (!value.HasValue)
            {
                return true;
            }

            return value.Value.IsEmpty();
        }

        //
        // 摘要:
        //     是否为空
        //
        // 参数:
        //   value:
        //     值
        public static bool IsEmpty(this Guid value)
        {
            if (value == Guid.Empty)
            {
                return true;
            }

            return false;
        }

        //
        // 摘要:
        //     是否为空
        //
        // 参数:
        //   value:
        //     值
        public static bool IsEmpty(this object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }

            return true;
        }

        //
        // 摘要:
        //     安全返回值
        //
        // 参数:
        //   value:
        //     可空值
        public static T SafeValue<T>(this T? value) where T : struct
        {
            return value.GetValueOrDefault();
        }

        //
        // 摘要:
        //     是否包含
        //
        // 参数:
        //   obj:
        //     字串
        //
        //   value:
        //     包含字串
        public static bool ContainsEx(this string obj, string value)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return false;
            }

            return obj.Contains(value);
        }

        //
        // 摘要:
        //     字串是否在指定字串中存在
        //
        // 参数:
        //   obj:
        //     字串
        //
        //   value:
        //     被包含字串
        public static bool Like(this string obj, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (string.IsNullOrEmpty(obj))
            {
                return false;
            }

            if (value.IndexOf(obj) != -1)
            {
                return true;
            }

            return false;
        }

        //
        // 摘要:
        //     数据字段类型转化
        //
        // 参数:
        //   str:
        //     字串
        public static DbType ToDbType(this string str)
        {
            DbType result = DbType.String;
            switch (str)
            {
                case "int?":
                    result = DbType.Int32;
                    break;
                case "byte?":
                    result = DbType.Byte;
                    break;
                case "float?":
                    result = DbType.Single;
                    break;
                case "decimal?":
                    result = DbType.Decimal;
                    break;
                case "string":
                    result = DbType.String;
                    break;
                case "bool?":
                    result = DbType.Boolean;
                    break;
                case "DateTime?":
                    result = DbType.DateTime;
                    break;
            }

            return result;
        }

    }
}
