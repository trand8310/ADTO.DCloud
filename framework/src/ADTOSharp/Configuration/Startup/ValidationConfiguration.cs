using System;
using System.Collections.Generic;
using ADTOSharp.Collections;
using ADTOSharp.Runtime.Validation.Interception;

namespace ADTOSharp.Configuration.Startup
{
    public class ValidationConfiguration : IValidationConfiguration
    {
        public List<Type> IgnoredTypes { get; }

        public ITypeList<IMethodParameterValidator> Validators { get; }

        public ValidationConfiguration()
        {
            IgnoredTypes = new List<Type>();
            Validators = new TypeList<IMethodParameterValidator>();
        }
    }
}