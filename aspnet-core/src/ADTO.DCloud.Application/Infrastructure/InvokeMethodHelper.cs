using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;

namespace ADTO.DCloud.Infrastructure
{
    public class InvokeMethodHelper : DCloudAppServiceBase
    {
        private readonly ConcurrentDictionary<string, Assembly> _assemblyCache = new();
        private readonly IServiceProvider _serviceProvider;
        public InvokeMethodHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #region MyRegion
        /// <summary>
        /// 判断服务是否存在并且已注册
        /// </summary>
        /// <param name="fullMethodName"></param>
        /// <returns></returns>
        public  bool IsInvokeByNameAsync(string fullMethodName)
        {
            var parts = fullMethodName.Split('.');
            string methodName = parts[^1];
            string serviceClassName = parts[^2];
            string namespaceName = string.Join(".", parts[..^2]);
            if (string.IsNullOrWhiteSpace(serviceClassName))
                return false;
            if (string.IsNullOrWhiteSpace(methodName))
                return false;
            // 1. 获取类型（不依赖完整命名空间）
            Type type = FindTypeByName(serviceClassName);
            if (type == null)
                return false;
            // 2. 获取服务实例
            object service = _serviceProvider.GetService(type);
            if (service == null)
                return false;
            // 3. 查找方法
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
                return false;
            return true;
        }
        public async Task<object> InvokeByNameAsync(string fullMethodName, params object[] parameters)
        {
            // 解析方法路径（格式：Namespace.ClassName.MethodName）
            var parts = fullMethodName.Split('.');
            //if (parts.Length < 3)
            //throw (new UserFriendlyException($"方法名格式必须为：Namespace.ClassName.MethodName"));

            string methodName = parts[^1];
            string serviceClassName = parts[^2];
            string namespaceName = string.Join(".", parts[..^2]);

            if (string.IsNullOrWhiteSpace(serviceClassName))
                throw new ArgumentNullException(nameof(serviceClassName));
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            // 1. 获取类型（不依赖完整命名空间）
            Type type = FindTypeByName(serviceClassName);
            if (type == null)
                throw new TypeLoadException($"找不到类型: {serviceClassName}");

            // 2. 获取服务实例
            object service = _serviceProvider.GetService(type);
            if (service == null)
                throw new InvalidOperationException($"服务未注册: {serviceClassName}");

            // 3. 查找方法
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
                throw new MissingMethodException($"找不到方法: {methodName}");

            // 4. 调用方法
            return await ExecuteMethodAsync(service, method, parameters);

        }

        private async Task<object> ExecuteMethodAsync(object instance, MethodInfo method, object[] parameters)
        {
            try
            {
                // 简单参数转换（不严格匹配）
                object[] convertedParams = method.GetParameters()
                    .Select((p, i) =>
                        (i < parameters?.Length) ? ConvertParameter(parameters[i], p.ParameterType) :
                        p.HasDefaultValue ? p.DefaultValue :
                        GetDefaultValue(p.ParameterType))
                    .ToArray();

                var result = method.Invoke(instance, convertedParams);

                // 处理异步方法
                if (result is System.Threading.Tasks.Task task)
                {
                    await task;
                    return task.GetType().IsGenericType ?
                        task.GetType().GetProperty("Result")?.GetValue(task) :
                        null;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"调用方法失败: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        private static object ConvertParameter(object rawValue, Type targetType)
        {
            if (rawValue == null)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            Type sourceType = rawValue.GetType();
            if (targetType == typeof(Guid) && rawValue is string strGuid)
                return Guid.Parse(strGuid);
            if (targetType == typeof(string) && rawValue is Guid guid)
                return guid.ToString();
            // 目标类型是字符串，但原始值是数组或集合（且不是字符串）
            if (targetType == typeof(string) && !(rawValue is string) && rawValue is IEnumerable enumerable)
            {
                // 方案1：转为 JSON 字符串（推荐，保留完整结构）
                return System.Text.Json.JsonSerializer.Serialize(rawValue);

                // 方案2：转为逗号分隔（适用于简单类型数组）
                // var items = enumerable.Cast<object>().Select(x => x?.ToString());
                // return string.Join(",", items);
            }

            // 处理 Nullable<T>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type innerType = targetType.GetGenericArguments()[0];
                return Convert.ChangeType(rawValue, innerType);
            }

            // 枚举转换
            if (targetType.IsEnum)
            {
                return Enum.ToObject(targetType, Convert.ChangeType(rawValue, Enum.GetUnderlyingType(targetType)));
            }

            // 常规类型转换
            try
            {
                return Convert.ChangeType(rawValue, targetType);
            }
            catch
            {
                // 如果转换失败，可以尝试 JSON 反序列化（复杂对象转目标类型）
                if (targetType.IsClass && targetType != typeof(string))
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(rawValue);
                    return System.Text.Json.JsonSerializer.Deserialize(json, targetType);
                }
                throw; // 或者返回默认值
            }
        }
        private object ConvertParameter_old(object value, Type targetType)
        {
            if (value == null || value.GetType() == targetType)
                return value;

            try
            {
                // 基础类型转换
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                // 尝试JSON序列化转换
                try
                {
                    string json = JsonSerializer.Serialize(value);
                    return JsonSerializer.Deserialize(json, targetType);
                }
                catch
                {
                    return value; // 最后尝试直接传参（可能抛出异常）
                }
            }
        }
        private object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private Type FindTypeByName(string className)
        {
            try
            {
                // 优先从已注册的服务中查找
                var serviceType = _serviceProvider.GetServices<object>()
                    .Select(s => s.GetType())
                    .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

                if (serviceType != null) return serviceType;
                // 从所有已加载程序集查找（性能警告：建议缓存结果）
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(asm => !asm.IsDynamic)
                    .SelectMany(asm => asm.GetTypes())
                    .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}

