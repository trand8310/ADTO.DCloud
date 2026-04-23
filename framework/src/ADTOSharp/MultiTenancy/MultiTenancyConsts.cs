using System;

namespace ADTOSharp.MultiTenancy
{
    public static class MultiTenancyConsts
    {
        /// <summary>
        /// Default tenant id: 00000000-0000-0000-0000-000000000001.
        /// </summary>
        public static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}