using System;
using System.Collections.Generic;

namespace ADTOSharp.Auditing
{
    public interface IADTOSharpAuditingDefaultOptions
    {
        /// <summary>
        /// A list of selectors to determine conventional Auditing classes.
        /// </summary>
        List<Func<Type, bool>> ConventionalAuditingSelectors { get; }
    }
}
