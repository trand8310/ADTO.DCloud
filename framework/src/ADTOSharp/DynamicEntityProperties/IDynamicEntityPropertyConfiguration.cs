using ADTOSharp.Collections;

namespace ADTOSharp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyConfiguration
    {
        ITypeList<DynamicEntityPropertyDefinitionProvider> Providers { get; }
    }
}
