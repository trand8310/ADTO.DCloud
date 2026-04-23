using System.Threading.Tasks;
using Medallion.Threading;

namespace ADTO.DistributedLocking;

public class MedallionADTODistributedLockHandle : IADTODistributedLockHandle
{
    public IDistributedSynchronizationHandle Handle { get; }

    public MedallionADTODistributedLockHandle(IDistributedSynchronizationHandle handle)
    {
        Handle = handle;
    }

    public ValueTask DisposeAsync()
    {
        return Handle.DisposeAsync();
    }
}
