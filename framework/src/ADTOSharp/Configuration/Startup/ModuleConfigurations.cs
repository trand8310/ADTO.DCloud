namespace ADTOSharp.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IADTOSharpStartupConfiguration ADTOSharpConfiguration { get; private set; }

        public ModuleConfigurations(IADTOSharpStartupConfiguration adtoConfiguration)
        {
            ADTOSharpConfiguration = adtoConfiguration;
        }
    }
}