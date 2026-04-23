using ADTOSharp.Configuration.Startup;
using ADTOSharp.Localization.Dictionaries;
using ADTOSharp.Localization.Dictionaries.Xml;
using ADTOSharp.Reflection.Extensions;

namespace ADTO.DCloud.Localization
{
    public static class DCloudLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(DCloudConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(DCloudLocalizationConfigurer).GetAssembly(),
                        "ADTO.DCloud.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}

