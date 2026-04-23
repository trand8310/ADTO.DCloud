using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results.Wrapping;

public interface IADTOSharpActionResultWrapperFactory : ITransientDependency
{
    IADTOSharpActionResultWrapper CreateFor(FilterContext context);
}