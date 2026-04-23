using System;
using System.Threading;

namespace ADTOSharp.Threading
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
        IDisposable Use(CancellationToken cancellationToken);
    }
}
