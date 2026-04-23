using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp.Localization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// 扩展.json序列反序列化
    /// </summary>
    public static class ExtensionsJson
    {
        //
        // 摘要:
        //     转成json对象
        //
        // 参数:
        //   Json:
        //     json字串
        public static object ToJson(this string Json)
        {
            return (Json == null) ? null : JsonConvert.DeserializeObject(Json);
        }

        //
        // 摘要:
        //     转成json字串
        //
        // 参数:
        //   obj:
        //     对象
        public static string ToJson(this object obj)
        {
            IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.SerializeObject(obj, isoDateTimeConverter);
        }

        //
        // 摘要:
        //     转成json字串
        //
        // 参数:
        //   obj:
        //     对象
        //
        //   datetimeformats:
        //     时间格式化
        public static string ToJson(this object obj, string datetimeformats)
        {
            IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = datetimeformats
            };
            return JsonConvert.SerializeObject(obj, isoDateTimeConverter);
        }

        //
        // 摘要:
        //     将一个对象转化成另一个对象
        //
        // 参数:
        //   obj:
        //     对象
        //
        // 类型参数:
        //   T:
        //     实体类型
        public static T ToObject<T>(this object obj)
        {
            return obj.ToJson().ToObject<T>();
        }

        //
        // 摘要:
        //     字串反序列化成指定对象实体
        //
        // 参数:
        //   Json:
        //     字串
        //
        // 类型参数:
        //   T:
        //     实体类型
        public static T ToObject<T>(this string Json)
        {
            return (T)((Json == null) ? ((object)default(T)) : ((object)JsonConvert.DeserializeObject<T>(Json)));
        }

        //
        // 摘要:
        //     字串反序列化成指定对象实体(列表)
        //
        // 参数:
        //   Json:
        //     字串
        //
        // 类型参数:
        //   T:
        //     实体类型
        public static List<T> ToList<T>(this string Json)
        {
            return (Json == null) ? null : JsonConvert.DeserializeObject<List<T>>(Json);
        }

        //
        // 摘要:
        //     字串反序列化成DataTable
        //
        // 参数:
        //   Json:
        //     字串
        public static DataTable ToTable(this string Json)
        {
            return (Json == null) ? null : JsonConvert.DeserializeObject<DataTable>(Json);
        }

        //
        // 摘要:
        //     字串反序列化成linq对象
        //
        // 参数:
        //   Json:
        //     字串
        public static JObject ToJObject(this string Json)
        {
            return (Json == null) ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }
        /// <summary>
        /// 转换函数
        /// 例如{"Identity.Id":"123456","Identity.Name":"测试."}
        /// 转换成{"Identity":{"Id":"123456","Name":"测试"}}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JObject ConvertToJson(Dictionary<string, string> data)
        {
            JObject result = new JObject();
            foreach (var pair in data)
            {
                // 分割路径（例如：AA.WelcomeMessage 分割为 ["AA", "WelcomeMessage"]）
                var parts = pair.Key.Split('.');
                // 获取字段名和对应的值
                var value = pair.Value;
                // 创建嵌套结构
                JObject current = result;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var key = parts[i];
                    // 如果当前 key 不存在，则创建它
                    if (current[key] == null)
                    {
                        current[key] = new JObject();
                    }
                    current = (JObject)current[key];
                }
                // 给最后一层的字段赋值
                current[parts[parts.Length - 1]] = value;
            }
            return result;
        }

    }
}
