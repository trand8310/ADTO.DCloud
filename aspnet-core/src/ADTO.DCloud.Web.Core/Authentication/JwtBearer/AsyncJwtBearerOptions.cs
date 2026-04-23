using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ADTO.DCloud.Web.Authentication.JwtBearer;

public class AsyncJwtBearerOptions : JwtBearerOptions
{
    public readonly List<IAsyncSecurityTokenValidator> AsyncSecurityTokenValidators;
    
    private readonly DCloudAsyncJwtSecurityTokenHandler _defaultAsyncHandler = new DCloudAsyncJwtSecurityTokenHandler();

    public AsyncJwtBearerOptions()
    {
        AsyncSecurityTokenValidators = new List<IAsyncSecurityTokenValidator>() {_defaultAsyncHandler};
    }
}

