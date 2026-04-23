namespace ADTOSharp.Configuration.Startup
{
    /// <summary>
    /// Used to provide a way to configure modules.
    /// Create entension methods to this class to be used over <see cref="IADTOSharpStartupConfiguration.Modules"/> object.
    /// </summary>
    public interface IModuleConfigurations
    {
        /// <summary>
        /// Gets the ADTO configuration object.
        /// </summary>
        IADTOSharpStartupConfiguration ADTOSharpConfiguration { get; }
    }
}