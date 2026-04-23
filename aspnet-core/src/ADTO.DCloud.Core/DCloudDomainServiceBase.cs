

using ADTOSharp.Domain.Services;

namespace ADTO.DCloud;

public abstract class DCloudDomainServiceBase : DomainService
{
    /* 添加常规领域服务 */

    protected DCloudDomainServiceBase()
    {
        LocalizationSourceName = DCloudConsts.LocalizationSourceName;
    }
}

