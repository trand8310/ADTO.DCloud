using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Authentication.External;

public interface IExternalAuthConfiguration
{
    [Obsolete("Use IExternalLoginInfoProviders")]
    List<ExternalLoginProviderInfo> Providers { get; }
    List<IExternalLoginInfoProvider> ExternalLoginInfoProviders { get; }
}
