using System;

namespace ADTO.DCloud.Net.Emailing
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(Guid? tenantId);
    }
}
