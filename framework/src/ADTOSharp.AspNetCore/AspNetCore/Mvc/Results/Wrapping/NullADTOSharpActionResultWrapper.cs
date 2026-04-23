using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results.Wrapping;

public class NullADTOSharpActionResultWrapper : IADTOSharpActionResultWrapper
{
    public void Wrap(FilterContext context)
    {

    }

}