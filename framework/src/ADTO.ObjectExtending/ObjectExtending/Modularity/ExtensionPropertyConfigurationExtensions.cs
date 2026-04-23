using ADTOSharp.Localization;
using System;
 
namespace ADTOSharp.ObjectExtending.Modularity;

public static class ExtensionPropertyConfigurationExtensions
{
    public static string? GetLocalizationResourceNameOrNull(
        this ExtensionPropertyConfiguration property)
    {
        if (property.DisplayName is LocalizableString localizableString)
        {
            return localizableString.Name;
        }

        return null;
    }

    public static Type? GetLocalizationResourceTypeOrNull(
        this ExtensionPropertyConfiguration property)
    {
        //if (property.DisplayName is LocalizableString localizableString)
        //{
        //    return localizableString.ResourceType;
        //}

        return null;
    }
}
