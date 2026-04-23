using System;
using ADTOSharp.Extensions;
using Medallion.Threading;

namespace ADTO.DistributedLocking;

public static class ADTODistributedLockHandleExtensions
{
    public static IDistributedSynchronizationHandle ToDistributedSynchronizationHandle(
        this IADTODistributedLockHandle handle)
    {
        return handle.As<MedallionADTODistributedLockHandle>().Handle;
    }
}
