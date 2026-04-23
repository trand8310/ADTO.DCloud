using ADTOSharp.AspNetCore.Mvc.Auditing;
using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTOSharp.AspNetCore.Mvc.Conventions;
using ADTOSharp.AspNetCore.Mvc.ExceptionHandling;
using ADTOSharp.AspNetCore.Mvc.ModelBinding;
using ADTOSharp.AspNetCore.Mvc.Results;
using ADTOSharp.AspNetCore.Mvc.Uow;
using ADTOSharp.AspNetCore.Mvc.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ADTOSharp.AspNetCore.Mvc;

internal static class ADTOSharpMvcOptionsExtensions
{
    public static void AddADTOSharp(this MvcOptions options, IServiceCollection services)
    {
        AddConventions(options, services);
        AddActionFilters(options);
        AddPageFilters(options);
        AddModelBinders(options);
    }

    private static void AddConventions(MvcOptions options, IServiceCollection services)
    {
        options.Conventions.Add(new ADTOSharpAppServiceConvention(services));
    }

    private static void AddActionFilters(MvcOptions options)
    {
        options.Filters.AddService(typeof(ADTOSharpAuthorizationFilter));
        options.Filters.AddService(typeof(ADTOSharpAuditActionFilter));
        options.Filters.AddService(typeof(ADTOSharpValidationActionFilter));
        options.Filters.AddService(typeof(ADTOSharpUowActionFilter));
        options.Filters.AddService(typeof(ADTOSharpExceptionFilter));
        options.Filters.AddService(typeof(ADTOSharpResultFilter));
    }

    private static void AddPageFilters(MvcOptions options)
    {
        options.Filters.AddService(typeof(ADTOSharpUowPageFilter));
        options.Filters.AddService(typeof(ADTOSharpAuditPageFilter));
        options.Filters.AddService(typeof(ADTOSharpResultPageFilter));
        options.Filters.AddService(typeof(ADTOSharpExceptionPageFilter));
    }

    private static void AddModelBinders(MvcOptions options)
    {
        options.ModelBinderProviders.Insert(0, new ADTOSharpDateTimeModelBinderProvider());
    }
}