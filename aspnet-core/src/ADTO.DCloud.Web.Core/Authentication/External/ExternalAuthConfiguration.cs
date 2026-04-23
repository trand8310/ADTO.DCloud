using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Authentication.External;

public class ExternalAuthConfiguration : IExternalAuthConfiguration, ISingletonDependency
{
    public ExternalAuthConfiguration()
    {
        ExternalLoginInfoProviders = new List<IExternalLoginInfoProvider>();
        Providers = new List<ExternalLoginProviderInfo>();
    }

    public List<ExternalLoginProviderInfo> Providers { get; }

    public List<IExternalLoginInfoProvider> ExternalLoginInfoProviders { get; }
}
