using ADTOSharp.Application.Navigation;
using ADTOSharp.Collections;

namespace ADTOSharp.Configuration.Startup
{
    internal class NavigationConfiguration : INavigationConfiguration
    {
        public ITypeList<NavigationProvider> Providers { get; private set; }

        public NavigationConfiguration()
        {
            Providers = new TypeList<NavigationProvider>();
        }
    }
}