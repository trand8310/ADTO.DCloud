using System;

namespace ADTOSharp.Runtime.Session
{
    public class SessionOverride
    {
        public Guid? UserId { get; }

        public Guid? TenantId { get; }

        public SessionOverride(Guid? tenantId, Guid? userId)
        {
            TenantId = tenantId;
            UserId = userId;
        }
    }
}