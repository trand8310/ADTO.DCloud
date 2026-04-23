using System;

namespace ADTO.DCloud.Chat
{
    public interface IChatFeatureChecker
    {
        void CheckChatFeatures(Guid? sourceTenantId, Guid? targetTenantId);
    }
}
