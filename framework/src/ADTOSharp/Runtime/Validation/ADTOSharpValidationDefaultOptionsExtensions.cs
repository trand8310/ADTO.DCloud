using System;
using System.Linq;

namespace ADTOSharp.Runtime.Validation
{
    internal static class ADTOSharpValidationOptionsExtensions
    {
        public static bool IsConventionalValidationClass(this IADTOSharpValidationDefaultOptions options, Type type)
        {
            return options.ConventionalValidationSelectors.Any(selector => selector(type));
        }
    }
}
