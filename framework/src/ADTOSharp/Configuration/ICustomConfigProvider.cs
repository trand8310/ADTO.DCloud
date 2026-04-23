using System.Collections.Generic;

namespace ADTOSharp.Configuration.Startup
{
    public interface ICustomConfigProvider
    {
        Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext);
    }
}
