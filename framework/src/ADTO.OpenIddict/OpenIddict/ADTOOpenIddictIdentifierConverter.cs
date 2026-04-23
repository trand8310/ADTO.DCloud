using ADTOSharp.Dependency;
using System;
 

namespace ADTO.OpenIddict;

public class ADTOOpenIddictIdentifierConverter : ITransientDependency
{
    public virtual Guid FromString(string identifier)
    {
        return string.IsNullOrEmpty(identifier) ? default : Guid.Parse(identifier);
    }

    public virtual string ToString(Guid identifier)
    {
        return identifier.ToString("D");
    }
}