using ADTOSharp.AspNetCore.Mvc.Results;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Reflection;

namespace ADTOSharp.AspNetCore.Mvc.Extensions;

public static class ActionDescriptorExtensions
{


    public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
    {
        if (!actionDescriptor.IsControllerAction())
        {
            throw new ADTOSharpException($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
        }

        return (actionDescriptor as ControllerActionDescriptor)!;
    }

    public static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
    }

    public static Type GetReturnType(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor.GetMethodInfo().ReturnType;
    }

    public static bool HasObjectResult(this ActionDescriptor actionDescriptor)
    {
        return ActionResultHelper.IsObjectResult(actionDescriptor.GetReturnType());
    }

    public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor is ControllerActionDescriptor;
    }

    public static bool IsPageAction(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor is PageActionDescriptor;
    }

    public static PageActionDescriptor AsPageAction(this ActionDescriptor actionDescriptor)
    {
        if (!actionDescriptor.IsPageAction())
        {
            throw new ADTOSharpException($"{nameof(actionDescriptor)} should be type of {typeof(PageActionDescriptor).AssemblyQualifiedName}");
        }

        return (actionDescriptor as PageActionDescriptor)!;
    }




    //public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
    //{
    //    if (!actionDescriptor.IsControllerAction())
    //    {
    //        throw new ADTOSharpException($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
    //    }

    //    return actionDescriptor as ControllerActionDescriptor;
    //}

    //public static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
    //{
    //    return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
    //}

    //public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
    //{
    //    return actionDescriptor is ControllerActionDescriptor;
    //}






}