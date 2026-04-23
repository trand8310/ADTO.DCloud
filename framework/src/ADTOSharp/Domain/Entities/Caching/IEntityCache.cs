using ADTOSharp.Runtime.Caching;

namespace ADTOSharp.Domain.Entities.Caching
{
    public interface IEntityCache<TCacheItem> : IEntityCache<TCacheItem, int>
    {
    }

    public interface IEntityCache<TCacheItem, TPrimaryKey> : IEntityCacheBase<TCacheItem, TPrimaryKey>
    {
        ITypedCache<TPrimaryKey, TCacheItem> InternalCache { get; }
    }
}