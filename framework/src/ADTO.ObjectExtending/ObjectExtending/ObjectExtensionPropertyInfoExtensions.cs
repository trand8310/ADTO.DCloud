using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ADTOSharp.ObjectExtending;

public static class ObjectExtensionPropertyInfoExtensions
{
    public static ValidationAttribute[] GetValidationAttributes(this ObjectExtensionPropertyInfo propertyInfo)
    {
        return propertyInfo
            .Attributes
            .OfType<ValidationAttribute>()
            .ToArray();
    }
}
