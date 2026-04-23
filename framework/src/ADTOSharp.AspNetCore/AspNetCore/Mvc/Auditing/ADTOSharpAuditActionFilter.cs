using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ADTOSharp.Aspects;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.Auditing;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Auditing;

public class ADTOSharpAuditActionFilter : IAsyncActionFilter, ITransientDependency
{
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;
    private readonly IAuditingHelper _auditingHelper;
    private readonly IAuditingConfiguration _auditingConfiguration;
    private readonly IAuditSerializer _auditSerializer;

    public ADTOSharpAuditActionFilter(IADTOSharpAspNetCoreConfiguration configuration,
        IAuditingHelper auditingHelper,
        IAuditingConfiguration auditingConfiguration,
        IAuditSerializer auditSerializer)
    {
        _configuration = configuration;
        _auditingHelper = auditingHelper;
        _auditingConfiguration = auditingConfiguration;
        _auditSerializer = auditSerializer;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!ShouldSaveAudit(context))
        {
            await next();
            return;
        }

        using (ADTOSharpCrossCuttingConcerns.Applying(context.Controller, ADTOSharpCrossCuttingConcerns.Auditing))
        {
            var auditInfo = _auditingHelper.CreateAuditInfo(
                context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
                context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                context.ActionArguments
            );

            var stopwatch = Stopwatch.StartNew();

            ActionExecutedContext result = null;
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
                            auditInfo.ReturnValue = _auditSerializer.Serialize(objectResult.Value);
                            break;

                        case JsonResult jsonResult:
                            auditInfo.ReturnValue = _auditSerializer.Serialize(jsonResult.Value);
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

    private bool ShouldSaveAudit(ActionExecutingContext actionContext)
    {
        return _configuration.IsAuditingEnabled &&
               actionContext.ActionDescriptor.IsControllerAction() &&
               _auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
    }
}