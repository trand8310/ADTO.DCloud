using System;
using System.Threading.Tasks;
using ADTOSharp.Timing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging.Abstractions;

namespace ADTOSharp.AspNetCore.Mvc.ModelBinding;

public class ADTOSharpDateTimeModelBinder : IModelBinder
{
    private readonly Type _type;
    private readonly SimpleTypeModelBinder _simpleTypeModelBinder;

    public ADTOSharpDateTimeModelBinder(Type type)
    {
        _type = type;
        _simpleTypeModelBinder = new SimpleTypeModelBinder(type, NullLoggerFactory.Instance);
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        await _simpleTypeModelBinder.BindModelAsync(bindingContext);

        if (!bindingContext.Result.IsModelSet)
        {
            return;
        }

        if (_type == typeof(DateTime))
        {
            var dateTime = (DateTime)bindingContext.Result.Model;
            bindingContext.Result = ModelBindingResult.Success(Clock.Normalize(dateTime));
        }
        else
        {
            var dateTime = (DateTime?)bindingContext.Result.Model;
            if (dateTime != null)
            {
                bindingContext.Result = ModelBindingResult.Success(Clock.Normalize(dateTime.Value));
            }
        }
    }
}