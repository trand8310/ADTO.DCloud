using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes
{
    public static class PermissionHelper
    {
        private static readonly ConcurrentDictionary<MethodInfo, string> _cache = new();

        public static string GetPermissionCode(MethodInfo method)
        {
            return _cache.GetOrAdd(method, m =>
            {
                var attr = m.GetCustomAttribute<DataAuthPermissionAttribute>();
                return attr?.Permission;
            });
        }

        // 表达式树版本
        public static string GetPermissionCode<T>(Expression<Action<T>> expression)
        {
            if (expression.Body is MethodCallExpression methodCall)
            {
                return GetPermissionCode(methodCall.Method);
            }
            return null;
        }
    }
}
