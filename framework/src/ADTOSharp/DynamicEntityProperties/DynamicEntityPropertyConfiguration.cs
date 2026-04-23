using ADTOSharp.Collections;

namespace ADTOSharp.DynamicEntityProperties
{
    public class DynamicEntityPropertyConfiguration : IDynamicEntityPropertyConfiguration
    {
        public ITypeList<DynamicEntityPropertyDefinitionProvider> Providers { get; }

        public DynamicEntityPropertyConfiguration()
        {
            Providers = new TypeList<DynamicEntityPropertyDefinitionProvider>();
        }
    }
}
