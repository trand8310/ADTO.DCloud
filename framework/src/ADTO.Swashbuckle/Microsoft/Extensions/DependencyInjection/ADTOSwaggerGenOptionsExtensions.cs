using System;
using System.Linq;
using ADTO.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class ADTOSwaggerGenOptionsExtensions
{
    public static void HideADTOEndpoints(this SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.DocumentFilter<ADTOSwashbuckleDocumentFilter>();
    }

    public static void UserFriendlyEnums(this SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.SchemaFilter<ADTOSwashbuckleEnumSchemaFilter>();
    }

    public static void CustomADTOSchemaIds(this SwaggerGenOptions options)
    {
        string SchemaIdSelector(Type modelType)
        {
            if (!modelType.IsConstructedGenericType)
            {
                return modelType.FullName!.Replace("[]", "Array");
            }

            var prefix = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .Aggregate((previous, current) => previous + current);
            return modelType.FullName!.Split('`').First() + "Of" + prefix;
        }

        options.CustomSchemaIds(SchemaIdSelector);
    }
}
