using System;
using ADTOSharp.Dependency;
using StackExchange.Redis;

namespace ADTOSharp.Runtime.Caching.Redis
{
    /// <summary>
    /// Implements <see cref="IADTOSharpRedisCacheDatabaseProvider"/>.
    /// </summary>
    public class ADTOSharpRedisCacheDatabaseProvider : IADTOSharpRedisCacheDatabaseProvider, ISingletonDependency
    {
        private readonly ADTOSharpRedisCacheOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ADTOSharpRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public ADTOSharpRedisCacheDatabaseProvider(ADTOSharpRedisCacheOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.DatabaseId);
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }
    }
}
