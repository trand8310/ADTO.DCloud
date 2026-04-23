using System.Collections.Generic;
using ADTOSharp.Localization;
using ADTOSharp.Web;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ADTOSharp.AspNetCore.Mvc.Models;

public static class ModelStateExtensions
{
    public static AjaxResponse ToMvcAjaxResponse(this ModelStateDictionary modelState, ILocalizationManager localizationManager)
    {
        if (modelState.IsValid)
        {
            return new AjaxResponse();
        }

        var validationErrors = new List<ValidationErrorInfo>();

        foreach (var state in modelState)
        {
            foreach (var error in state.Value.Errors)
            {
                validationErrors.Add(new ValidationErrorInfo(error.ErrorMessage, state.Key));
            }
        }

        var errorInfo = new ErrorInfo(localizationManager.GetString(ADTOSharpWebConsts.LocalizationSourceName, "ValidationError"))
        {
            ValidationErrors = validationErrors.ToArray()
        };

        return new AjaxResponse(errorInfo);
    }
}