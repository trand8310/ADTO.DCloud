using System;
using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Application.Services;

namespace ADTOSharp.Runtime.Validation
{
    public class ADTOSharpValidationDefaultOptions : IADTOSharpValidationDefaultOptions
    {
        public static List<Func<Type, bool>> ConventionalValidationSelectorList = new List<Func<Type, bool>>
        {
            type => typeof(IApplicationService).IsAssignableFrom(type)
        };
        
        public List<Func<Type, bool>> ConventionalValidationSelectors { get; }

        public ADTOSharpValidationDefaultOptions()
        {
            ConventionalValidationSelectors = ConventionalValidationSelectorList.ToList();
        }
    }
}
