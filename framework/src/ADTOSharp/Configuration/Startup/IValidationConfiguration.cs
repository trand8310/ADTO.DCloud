using System;
using System.Collections.Generic;
using ADTOSharp.Collections;
using ADTOSharp.Runtime.Validation.Interception;

namespace ADTOSharp.Configuration.Startup
{
    public interface IValidationConfiguration
    {
        List<Type> IgnoredTypes { get; }

        /// <summary>
        /// A list of method parameter validators.
        /// </summary>
        ITypeList<IMethodParameterValidator> Validators { get; }
    }
}