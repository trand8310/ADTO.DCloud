using System;
using System.Threading.Tasks;

namespace ADTO.DistributedLocking;

public class LocalADTODistributedLockHandle : IADTODistributedLockHandle
{
    private readonly IDisposable _disposable;

    public LocalADTODistributedLockHandle(IDisposable disposable)
    {
        _disposable = disposable;
    }

    public ValueTask DisposeAsync()
    {
        _disposable.Dispose();
        return default;
    }
}
