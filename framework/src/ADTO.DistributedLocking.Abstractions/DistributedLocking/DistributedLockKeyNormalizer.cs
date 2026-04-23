using ADTOSharp.Dependency;
using Microsoft.Extensions.Options;

namespace ADTO.DistributedLocking;

public class DistributedLockKeyNormalizer : IDistributedLockKeyNormalizer, ITransientDependency
{
    protected ADTODistributedLockOptions Options { get; }
    
    public DistributedLockKeyNormalizer(IOptions<ADTODistributedLockOptions> options)
    {
        Options = options.Value;
    }
    
    public virtual string NormalizeKey(string name)
    {
        return $"{Options.KeyPrefix}{name}";
    }
}