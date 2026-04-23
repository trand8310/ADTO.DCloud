using System;
using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Application.Services;

namespace ADTOSharp.Auditing
{
    public class ADTOSharpAuditingDefaultOptions : IADTOSharpAuditingDefaultOptions
    {
        public static List<Func<Type, bool>> ConventionalAuditingSelectorList = new List<Func<Type, bool>>
        {
            type => typeof(IApplicationService).IsAssignableFrom(type)
        };

        public List<Func<Type, bool>> ConventionalAuditingSelectors { get; }

        public ADTOSharpAuditingDefaultOptions()
        {
            ConventionalAuditingSelectors = ConventionalAuditingSelectorList.ToList();
        }
    }
}
