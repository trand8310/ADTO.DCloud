using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        private object ConvertParameter(object value, Type targetType)
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

        public Type GetType1(string typeName)
        {
            // 首先尝试从已加载程序集查找
            var type = Type.GetType(typeName);
            if (type != null) return type;

            // 尝试解析程序集名称并加载
            var assemblyName = typeName.Split(',')[1].Trim();
            var assembly = LoadAssembly(assemblyName);
            return assembly?.GetType(typeName.Split(',')[0]);
        }

        public Type GetType2(string namespaceName, string className)
        {
            var fullName = $"{namespaceName}.{className}";

            // 1. 从已加载程序集查找
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullName);
                if (type != null) return type;
            }

            // 2. 尝试加载程序集
            var assemblyName = namespaceName.Split('.')[0];
            var loadedAssembly = LoadAssembly(assemblyName);
            return loadedAssembly?.GetType(fullName);
        }

        public Assembly LoadAssembly(string assemblyName)
        {
            return _assemblyCache.GetOrAdd(assemblyName, name =>
            {
                try
                {
                    // 尝试通过名称加载
                    return Assembly.Load(new AssemblyName(name));
                }
                catch
                {
                    // 尝试从文件加载
                    var path = Path.Combine(AppContext.BaseDirectory, $"{name}.dll");
                    return File.Exists(path) ? Assembly.LoadFrom(path) : null;
                }
            });
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

