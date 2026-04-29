using ADTO.DCloud.DataAuthorizes.Dto;
using ADTO.DCloud.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Infrastructure
{
    /// <summary>
    /// 反射方法dto-不执行接口
    /// </summary>-
    public class DtoMetadataHelper
    {

        /// <summary>
        /// 根据服务类型和方法名（以及可选的参数类型数组，用于处理重载）获取方法返回的 DTO 的属性与备注
        /// </summary>
        /// <param name="serviceType">服务类型（例如 typeof(UserAppService)）</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="parameterTypes">可选：参数类型数组，用于精确匹配重载方法</param>
        /// <returns>字典，键为属性名，值为备注文本</returns>
        public static List<PropertiesDto> GetDtoPropertiesWithRemarks(
            Type serviceType,
            string methodName,
            params Type[] parameterTypes)
        {
            // 获取方法信息
            MethodInfo method = GetMethod(serviceType, methodName, parameterTypes);
            if (method == null)
                throw new MissingMethodException(serviceType.FullName, methodName);

            // 提取内部的 DTO 类型
            Type dtoType = ExtractDtoType(method.ReturnType);
            if (dtoType == null)
                throw new InvalidOperationException($"无法从方法 {methodName} 的返回类型中提取 DTO 类型。返回类型：{method.ReturnType}");

            // 获取 DTO 的所有公共实例属性及其备注
            var result = new List<PropertiesDto>();
            foreach (var prop in dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string remark = GetRemarkFromAttributes(prop);
                result.Add(new PropertiesDto() { Field = prop.Name, Remark = remark });
                //result[prop.Name] = remark;
            }
            return result;
        }

        /// <summary>
        /// 获取方法信息（支持重载）
        /// </summary>
        private static MethodInfo GetMethod(Type serviceType, string methodName, Type[] parameterTypes)
        {
            if (parameterTypes == null || parameterTypes.Length == 0)
            {
                // 无参数或未指定参数类型，尝试获取第一个匹配的方法（注意可能有重载歧义）
                var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                         .Where(m => m.Name == methodName)
                                         .ToList();
                if (methods.Count == 0) return null;
                if (methods.Count > 1)
                    throw new AmbiguousMatchException($"方法 {methodName} 存在多个重载，请提供参数类型数组以精确匹配。");
                return methods[0];
            }
            else
            {
                // 根据参数类型精确匹配
                return serviceType.GetMethod(methodName, parameterTypes);
            }
        }

        /// <summary>
        /// 从方法的返回类型中提取最终的业务 DTO 类型。
        /// 支持解包：Task<T>、ValueTask<T>、PagedResultDto<T> 等常见泛型包装。
        /// </summary>
        private static Type ExtractDtoType(Type returnType)
        {
            // 1. 处理 Task<T> / ValueTask<T>
            if (returnType.IsGenericType)
            {
                Type genericDef = returnType.GetGenericTypeDefinition();
                if (genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>))
                {
                    return ExtractDtoType(returnType.GetGenericArguments()[0]);
                }
            }
            // 2. 处理自定义泛型包装类型：PagedResultWithAuthDto<T>, PagedResult<T>, Response<T> 等
            // 如果当前类型是泛型类，并且它的命名空间/名称符合包装特征，可以递归取第一个泛型参数
            if (returnType.IsGenericType && !returnType.IsGenericTypeDefinition)
            {
                // 这里可以配置一组已知的包装类型，或直接假设任何单一泛型参数的类型都是包装
                // 更严谨的做法：检查类型名称包含 "Result", "Response", "Dto" 等，或者通过特性标记
                var typeArgs = returnType.GetGenericArguments();
                if (typeArgs.Length == 1)
                {
                    // 递归解包，直到非泛型或非单一泛型参数
                    return ExtractDtoType(typeArgs[0]);
                }
            }

            // 3. 如果已经是非泛型类型，或者解包到最后，直接返回
            return returnType;
        }
        /// <summary>
        /// 辅助方法：判断是否为“真正的” DTO 类型（可根据特性、命名空间等自定义）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsDtoType(Type type)
        {
            // 示例：排除基元类型、字符串、值类型等
            if (type.IsPrimitive || type == typeof(string) || type.IsValueType)
                return false;
            // 也可以检查是否有自定义特性，如 [Dto]
            return true;
        }
        /// <summary>
        /// 从属性上读取备注，优先级：DisplayAttribute.Description > DisplayAttribute.Name > DescriptionAttribute.Description > "无备注"
        /// </summary>
        private static string GetRemarkFromAttributes(PropertyInfo property)
        {
            // DisplayAttribute
            var display = property.GetCustomAttribute<DisplayAttribute>();
            if (display != null)
            {
                if (!string.IsNullOrEmpty(display.Description))
                    return display.Description;
                if (!string.IsNullOrEmpty(display.Name))
                    return display.Name;
            }
            // DescriptionAttribute
            var desc = property.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null && !string.IsNullOrEmpty(desc.Description))
                return desc.Description;

            return "";
        }
    }
}
