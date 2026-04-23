using System;

namespace ADTO.DCloud.Url;
public class NullAppUrlService : IAppUrlService
{
    public static IAppUrlService Instance { get; } = new NullAppUrlService();

    private NullAppUrlService()
    {

    }

    public string CreateEmailActivationUrlFormat(Guid? tenantId)
    {
        throw new NotImplementedException();
    }

    public string CreateEmailChangeRequestUrlFormat(Guid? tenantId)
    {
        throw new NotImplementedException();
    }

    public string CreatePasswordResetUrlFormat(Guid? tenantId)
    {
        throw new NotImplementedException();
    }

    public string CreateEmailActivationUrlFormat(string tenancyName)
    {
        throw new NotImplementedException();
    }

    public string CreatePasswordResetUrlFormat(string tenancyName)
    {
        throw new NotImplementedException();
    }
}