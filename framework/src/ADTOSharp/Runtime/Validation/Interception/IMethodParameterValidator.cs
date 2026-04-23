using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Dependency;

namespace ADTOSharp.Runtime.Validation.Interception
{
    /// <summary>
    /// This interface is used to validate method parameters.
    /// </summary>
    public interface IMethodParameterValidator : ITransientDependency
    {
        IReadOnlyList<ValidationResult> Validate(object validatingObject);
    }
}
