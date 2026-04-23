using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTOSharp.Localization;

public static class HasNameWithLocalizableDisplayNameExtensions
{
    [NotNull]
    public static string GetLocalizedDisplayName(
        [NotNull] this IHasNameWithLocalizableDisplayName source,
        [NotNull] IStringLocalizerFactory stringLocalizerFactory,
        string? localizationNamePrefix = "DisplayName:")
    {
        //还未实现
        return source.Name ;

        //if (source.DisplayName != null)
        //{
        //    return source.DisplayName.Localize(stringLocalizerFactory);
        //}

        //var defaultStringLocalizer = stringLocalizerFactory.CreateDefaultOrNull();
        //if (defaultStringLocalizer == null)
        //{
        //    return source.Name;
        //}

        //var localizedString = defaultStringLocalizer[$"{localizationNamePrefix}{source.Name}"];
        //if (!localizedString.ResourceNotFound ||
        //    localizationNamePrefix.IsNullOrEmpty())
        //{
        //    return localizedString;
        //}

        //return defaultStringLocalizer[source.Name];
    }
}
