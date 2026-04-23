using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.Threading;
using Medallion.Threading;

namespace ADTO.DistributedLocking;

public class MedallionADTODistributedLock : IADTODistributedLock, ITransientDependency
{
    protected IDistributedLockProvider DistributedLockProvider { get; }
    protected ICancellationTokenProvider CancellationTokenProvider { get; }
    
    protected IDistributedLockKeyNormalizer DistributedLockKeyNormalizer { get; }

    public MedallionADTODistributedLock(
        IDistributedLockProvider distributedLockProvider,
        ICancellationTokenProvider cancellationTokenProvider,
        IDistributedLockKeyNormalizer distributedLockKeyNormalizer)
    {
        DistributedLockProvider = distributedLockProvider;
        CancellationTokenProvider = cancellationTokenProvider;
        DistributedLockKeyNormalizer = distributedLockKeyNormalizer;
    }

    public async Task<IADTODistributedLockHandle?> TryAcquireAsync(
        string name,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var key = DistributedLockKeyNormalizer.NormalizeKey(name);
        
        CancellationTokenProvider.FallbackToProvider(cancellationToken);

        var handle = await DistributedLockProvider.TryAcquireLockAsync(
            key,
            timeout,
            CancellationTokenProvider.FallbackToProvider(cancellationToken)
        );
        
        if (handle == null)
        {
            return null;
        }

        return new MedallionADTODistributedLockHandle(handle);
    }
}
