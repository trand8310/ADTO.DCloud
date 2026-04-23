using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ADTO.DCloud.Infrastructure
{
    public class DtoMetadataExtractor
    {
        /// <summary>
        /// 根据方法信息提取其返回的 DTO 类型的所有属性及备注
        /// </summary>
        /// <param name="method">MethodInfo 对象</param>
        /// <returns>属性名称和备注的字典</returns>
        public static Dictionary<string, string> GetPropertiesWithRemarks(MethodInfo method)
        {
            // 1. 获取方法的返回类型
            Type returnType = method.ReturnType;
            // 2. 解包异步方法 (Task<T>)
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            // 3. 解包 PagedResultDto<T>（假设它的定义是 class PagedResultDto<T>，包含 Items 或 Data 属性）
            Type dtoType = ExtractInnerDtoType(returnType);
            if (dtoType == null)
            {
                throw new InvalidOperationException("无法从返回类型中提取到 DTO 类型");
            }
            // 4. 获取 DTO 的所有公共实例属性
            PropertyInfo[] properties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = new Dictionary<string, string>();
            foreach (var prop in properties)
            {
                string remark = GetPropertyRemark(prop);
                result[prop.Name] = remark;
            }
            return result;
        }

        /// <summary>
        /// 从可能的泛型包装类型中提取内部的 DTO 类型（例如 PagedResultDto<AdtoAbsDto> -> AdtoAbsDto）
        /// </summary>
        private static Type ExtractInnerDtoType(Type type)
        {
            // 如果本身就是需要的 DTO（非泛型），直接返回
            if (!type.IsGenericType)
                return type;

            // 处理 PagedResultDto<T>：通常它的泛型参数就是 DTO 类型
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef.Name == "PagedResultDto`1") // 或使用 full name 判断
            {
                return type.GetGenericArguments()[0];
            }

            // 如果还有其它包装（例如 List<T>、IEnumerable<T>），可以递归处理
            // 这里简单返回第一个泛型参数
            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// 获取属性的备注信息（优先从特性中读取，否则尝试从 XML 注释读取）
        /// </summary>
        private static string GetPropertyRemark(PropertyInfo property)
        {
            // 方式1：从 DisplayAttribute 获取 Name 或 Description
            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null)
            {
                if (!string.IsNullOrEmpty(displayAttr.Description))
                    return displayAttr.Description;
                if (!string.IsNullOrEmpty(displayAttr.Name))
                    return displayAttr.Name;
            }

            // 方式2：从 DescriptionAttribute 获取
            var descAttr = property.GetCustomAttribute<DescriptionAttribute>();
            if (descAttr != null && !string.IsNullOrEmpty(descAttr.Description))
                return descAttr.Description;

            // 方式3：从 XML 注释文件读取（需要配置生成 XML 文档文件）
            string xmlRemark = GetRemarkFromAttributes(property);
            if (!string.IsNullOrEmpty(xmlRemark))
                return xmlRemark;

            return "";
        }
        private static string GetRemarkFromAttributes(PropertyInfo prop)
        {
            // DisplayAttribute
            var display = prop.GetCustomAttribute<DisplayAttribute>();
            if (display != null)
            {
                if (!string.IsNullOrEmpty(display.Description))
                    return display.Description;
                if (!string.IsNullOrEmpty(display.Name))
                    return display.Name;
            }

            // DescriptionAttribute
            var desc = prop.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null && !string.IsNullOrEmpty(desc.Description))
                return desc.Description;

            return "";
        }
    }
}
