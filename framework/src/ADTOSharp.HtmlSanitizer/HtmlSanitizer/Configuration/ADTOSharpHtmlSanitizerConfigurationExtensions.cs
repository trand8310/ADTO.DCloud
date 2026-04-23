using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.HtmlSanitizer.Configuration;

/// <summary>
/// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ADTO HTML sanitizer module.
/// </summary>
public static class ADTOSharpHtmlSanitizerConfigurationExtensions
{
    /// <summary>
    /// Used to configure ADTO HTML sanitizer module.
    /// </summary>
    public static IHtmlSanitizerConfiguration ADTOSharpHtmlSanitizer(this IModuleConfigurations configurations)
    {
        return configurations.ADTOSharpConfiguration.Get<IHtmlSanitizerConfiguration>();
    }
}