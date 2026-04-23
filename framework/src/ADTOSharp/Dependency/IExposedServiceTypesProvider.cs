using System;

namespace ADTOSharp.Dependency;

public interface IExposedServiceTypesProvider
{
    Type[] GetExposedServiceTypes(Type targetType);
}
