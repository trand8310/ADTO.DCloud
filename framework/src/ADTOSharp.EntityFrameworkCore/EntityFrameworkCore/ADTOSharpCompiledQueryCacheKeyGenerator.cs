using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

namespace ADTOSharp.EntityFrameworkCore;

public class ADTOSharpCompiledQueryCacheKeyGenerator : ICompiledQueryCacheKeyGenerator
{
    protected ICompiledQueryCacheKeyGenerator InnerCompiledQueryCacheKeyGenerator { get; }
    protected ICurrentDbContext CurrentContext { get; }

    public ADTOSharpCompiledQueryCacheKeyGenerator(
        ICompiledQueryCacheKeyGenerator innerCompiledQueryCacheKeyGenerator,
        ICurrentDbContext currentContext)
    {
        InnerCompiledQueryCacheKeyGenerator = innerCompiledQueryCacheKeyGenerator;
        CurrentContext = currentContext;
    }

    public virtual object GenerateCacheKey(Expression query, bool async)
    {
        var cacheKey = InnerCompiledQueryCacheKeyGenerator.GenerateCacheKey(query, async);
        if (CurrentContext.Context is ADTOSharpDbContext dbContext)
        {
            return new ADTOSharpCompiledQueryCacheKey(cacheKey, dbContext.GetCompiledQueryCacheKey());
        }

        return cacheKey;
    }

    private readonly struct ADTOSharpCompiledQueryCacheKey : IEquatable<ADTOSharpCompiledQueryCacheKey>
    {
        private readonly object _compiledQueryCacheKey;
        private readonly string _currentFilterCacheKey;

        public ADTOSharpCompiledQueryCacheKey(object compiledQueryCacheKey, string currentFilterCacheKey)
        {
            _compiledQueryCacheKey = compiledQueryCacheKey;
            _currentFilterCacheKey = currentFilterCacheKey;
        }

        public override bool Equals(object obj)
        {
            return obj is ADTOSharpCompiledQueryCacheKey key && Equals(key);
        }

        public bool Equals(ADTOSharpCompiledQueryCacheKey other)
        {
            return _compiledQueryCacheKey.Equals(other._compiledQueryCacheKey) &&
                   _currentFilterCacheKey == other._currentFilterCacheKey;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_compiledQueryCacheKey, _currentFilterCacheKey);
        }
    }
}