using System.Threading;
using ADTOSharp.Runtime.Remoting;

namespace ADTOSharp.Threading
{
    public class NullCancellationTokenProvider : CancellationTokenProviderBase
    {
        public static NullCancellationTokenProvider Instance { get; } = new NullCancellationTokenProvider();

        public override CancellationToken Token => CancellationToken.None;

        private NullCancellationTokenProvider()
        : base(
            new DataContextAmbientScopeProvider<CancellationTokenOverride>(new AsyncLocalAmbientDataContext()))
        {
        }
    }
}
