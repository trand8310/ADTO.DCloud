using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.WorkFlow.Processs;
using ADTO.DCloud.WorkFlow.Tasks.Dto;
using ADTOSharp.Application.Services.Dto;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes
{
    public class DataAuthInterceptor : IInterceptor
    {
        private readonly IDataFilterService _dataFilterService;
        private readonly ILogger<DataAuthInterceptor> _logger;

        public DataAuthInterceptor(ILogger<DataAuthInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            _logger.LogInformation($"拦截器开始处理: {invocation.Method.Name}");

            // 检查方法是否有DataAuth特性
            //var attr = invocation.Method.GetCustomAttribute<DataAuthPermissionAttribute>();
            string permissionCode = GetPermissionCodeByMethed(invocation.Method);

            //if (attr == null)
            //{
            //    invocation.Proceed(); // 无特性不拦截
            //    return;
            //}

            //// 执行原方法
            //invocation.Proceed();

            //// 处理返回结果
            //if (invocation.ReturnValue is IQueryable queryable)
            //{
            //    invocation.ReturnValue = _dataFilterService
            //        .CreateDataFilteredQuery((dynamic)queryable, attr.Permission)
            //        .GetAwaiter().GetResult();
            //}
            //else if (invocation.ReturnValue is Task task)
            //{
            //    invocation.ReturnValue = HandleAsyncResult(task, attr.Permission);
            //}
        }

        private async Task<object> HandleAsyncResult(Task task, string resourceType)
        {
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            if (resultProperty == null) return task;

            var result = resultProperty.GetValue(task);
            if (result is IQueryable queryableResult)
            {
                var filtered = await _dataFilterService
                    .CreateDataFilteredQuery((dynamic)queryableResult, resourceType);

                return typeof(Task).GetMethod("FromResult")!
                    .MakeGenericMethod(queryableResult.ElementType)
                    .Invoke(null, new[] { filtered });
            }

            return task;
        }
        // 私有方法，只供本拦截器使用
        private string GetPermissionCodeByMethed(MethodInfo method)
        {
            var attr = method.GetCustomAttribute<DataAuthPermissionAttribute>();
            return attr?.Permission;
        }
        /// <summary>
        /// 获取方法属性名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private string GetPermissionCode<T>(Expression<Action<T>> expression)
        {
            if (expression.Body is MethodCallExpression methodCall)
            {
                var methodInfo = methodCall.Method;
                var attribute = methodInfo.GetCustomAttribute<DataAuthPermissionAttribute>();
                return attribute?.Permission;
            }
            return null;
        }
        #region 以下是拦截服务，上面是根据特性拦截方法函数
        //private readonly IDataFilterService _dataFilterService;
        //private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
        //public DataAuthInterceptor(IDataFilterService dataFilterService)
        //{
        //    _dataFilterService = dataFilterService;
        //}

        //public void Intercept(IInvocation invocation)
        //{
        //    // 1. 检查方法是否有DataAuth特性
        //    var dataAuthAttr = invocation.Method.GetCustomAttribute<DataAuthAttribute>();

        //    if (dataAuthAttr == null)
        //    {
        //        invocation.Proceed();
        //        return;
        //    }
        //    // 处理同步方法
        //    if (invocation.ReturnValue is IQueryable queryable)
        //    {
        //        invocation.Proceed();
        //        invocation.ReturnValue = _dataFilterService.CreateDataFilteredQuery(
        //            (dynamic)invocation.ReturnValue,
        //            dataAuthAttr.ResourceType)
        //            .GetAwaiter().GetResult();
        //    }
        //    // 处理异步方法
        //    else if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
        //    {
        //        invocation.Proceed();

        //        if (invocation.ReturnValue is Task task)
        //        {
        //            invocation.ReturnValue = HandleAsyncMethod(
        //                task,
        //                invocation.Method.ReturnType,
        //                dataAuthAttr.ResourceType);
        //        }
        //    }
        //    else
        //    {
        //        invocation.Proceed();
        //    }
        //}
        //private async Task<object> HandleAsyncMethod(Task task,Type returnType,string resourceType)
        //{
        //    await task.ConfigureAwait(false);
        //    if (returnType.IsGenericType)
        //    {
        //        var resultProperty = returnType.GetProperty("Result");
        //        var result = resultProperty?.GetValue(task);
        //        if (result is IQueryable queryableResult)
        //        {
        //            var filteredQuery = await _dataFilterService.CreateDataFilteredQuery((dynamic)queryableResult,resourceType);
        //            // 重新创建Task
        //            return typeof(Task).GetMethod("FromResult")
        //                ?.MakeGenericMethod(returnType.GenericTypeArguments[0])
        //                .Invoke(null, new[] { filteredQuery });
        //        }
        //        else if (result is PagedResultDto<WorkFlowTaskDto> pagedResult)
        //        {
        //            var filteredQuery = await _dataFilterService.CreateDataFilteredQuery(
        //                pagedResult.Items.AsQueryable(),
        //                resourceType);

        //            pagedResult.Items = ((IQueryable<WorkFlowTaskDto>)filteredQuery).ToList();
        //            pagedResult.TotalCount = pagedResult.Items.Count;

        //            return typeof(Task).GetMethod("FromResult")
        //                ?.MakeGenericMethod(returnType.GenericTypeArguments[0])
        //                .Invoke(null, new[] { pagedResult });
        //        }
        //    }

        //    return task;
        //}
        #endregion

    }
}
