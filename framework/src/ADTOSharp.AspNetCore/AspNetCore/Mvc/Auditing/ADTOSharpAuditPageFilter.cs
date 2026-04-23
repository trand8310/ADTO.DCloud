using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ADTOSharp.Aspects;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.Auditing;
using ADTOSharp.Dependency;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Auditing;

public class ADTOSharpAuditPageFilter : IAsyncPageFilter, ITransientDependency
{
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;
    private readonly IAuditingHelper _auditingHelper;
    private readonly IAuditingConfiguration _auditingConfiguration;
    private readonly IAuditSerializer _auditSerializer;

    public ADTOSharpAuditPageFilter(IADTOSharpAspNetCoreConfiguration configuration,
        IAuditingHelper auditingHelper,
        IAuditingConfiguration auditingConfiguration,
        IAuditSerializer auditSerializer)
    {
        _configuration = configuration;
        _auditingHelper = auditingHelper;
        _auditingConfiguration = auditingConfiguration;
        _auditSerializer = auditSerializer;
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (context.HandlerMethod == null || !ShouldSaveAudit(context))
        {
            await next();
            return;
        }

        using (ADTOSharpCrossCuttingConcerns.Applying(context.HandlerInstance, ADTOSharpCrossCuttingConcerns.Auditing))
        {
            var auditInfo = _auditingHelper.CreateAuditInfo(
                context.HandlerInstance.GetType(),
                context.HandlerMethod.MethodInfo,
                context.GetBoundPropertiesAsDictionary()
            );

            var stopwatch = Stopwatch.StartNew();

            PageHandlerExecutedContext result = null;
            try
            {
                result = await next();
                if (result.Exception != null && !result.ExceptionHandled)
                {
                    auditInfo.Exception = result.Exception;
                }
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                if (_auditingConfiguration.SaveReturnValues && result != null)
                {
                    switch (result.Result)
                    {
                        case ObjectResult objectResult:
                            if (objectResult.Value is AjaxResponse ajaxObjectResponse)
                            {
                                auditInfo.ReturnValue = _auditSerializer.Serialize(ajaxObjectResponse.Result);
                            }
                            else
                            {
                                auditInfo.ReturnValue = _auditSerializer.Serialize(objectResult.Value);
                            }
                            break;

                        case JsonResult jsonResult:
                            if (jsonResult.Value is AjaxResponse ajaxJsonResponse)
                            {
                                auditInfo.ReturnValue = _auditSerializer.Serialize(ajaxJsonResponse.Result);
                            }
                            else
                            {
                                auditInfo.ReturnValue = _auditSerializer.Serialize(jsonResult.Value);
                            }
                            break;

                        case ContentResult contentResult:
                            auditInfo.ReturnValue = contentResult.Content;
                            break;
                    }
                }

                await _auditingHelper.SaveAsync(auditInfo);
            }
        }
    }

    private bool ShouldSaveAudit(PageHandlerExecutingContext actionContext)
    {
        return _configuration.IsAuditingEnabled &&
               _auditingHelper.ShouldSaveAudit(actionContext.HandlerMethod.MethodInfo, true);
    }
}