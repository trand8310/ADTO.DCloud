using ADTOSharp.Dependency;
using ADTOSharp.RealTime;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Immutable;
using System.Text.Json;
using ADTOSharp.Json;
using System.Threading;
using ADTOSharp.Extensions;



namespace ADTOSharp.Runtime.Caching.Redis.RealTime
{
    public class RedisOnlineClientStore : IOnlineClientStore, ISingletonDependency
    {
        private readonly IADTOSharpRedisCacheDatabaseProvider _database;
        private readonly string _clientStoreKey;
        private readonly string _userStoreKey;
        private readonly string _heartbeatKeyPrefix;
        private readonly string _instanceStoreKeyPrefix;
        private readonly int _heartbeatTtlSeconds;

        public string InstanceId { get; }

        public RedisOnlineClientStore(
            IADTOSharpRedisCacheDatabaseProvider database,
            ADTOSharpRedisCacheOptions options)
        {
            _database = database;
            _clientStoreKey = options.OnlineClientsStoreKey + ".Clients";
            _userStoreKey = options.OnlineClientsStoreKey + ".Users";
            _heartbeatKeyPrefix = options.OnlineClientHeartbeatKeyPrefix;
            _instanceStoreKeyPrefix = options.OnlineClientInstanceStoreKeyPrefix;
            _heartbeatTtlSeconds = Math.Max(30, options.OnlineClientHeartbeatTtlSeconds);
            InstanceId = OnlineClientInstanceIdProvider.GetInstanceId();
        }

        public async Task AddAsync(IOnlineClient client)
        {
            var database = GetDatabase();
            var userIdentifier = client.ToUserIdentifierOrNull();

            const string script = @"
            redis.call('SADD', KEYS[1], ARGV[1]);
            redis.call('HSET', KEYS[2], ARGV[1], ARGV[2]);
            redis.call('SET', KEYS[3], ARGV[3], 'EX', ARGV[4]);
            redis.call('SADD', KEYS[4], ARGV[1]);
            return 1;";

            client.Properties ??= new Dictionary<string, object>();
            client["instanceId"] = InstanceId;

            var heartbeatKey = GetHeartbeatKey(client.ConnectionId);
            var instanceConnectionsKey = GetInstanceConnectionsKey(InstanceId);

            if (userIdentifier != null)
            {
                var userConnectionsKey = GetUserConnectionsKey(userIdentifier);
                await database.ScriptEvaluateAsync(script,
                    new RedisKey[] { userConnectionsKey, _clientStoreKey, heartbeatKey, instanceConnectionsKey },
                    new RedisValue[] { client.ConnectionId, client.ToJsonString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds(), _heartbeatTtlSeconds });
            }
            else
            {
                const string anonymousClientScript = @"
                redis.call('HSET', KEYS[1], ARGV[1], ARGV[2]);
                redis.call('SET', KEYS[2], ARGV[3], 'EX', ARGV[4]);
                redis.call('SADD', KEYS[3], ARGV[1]);
                return 1;";

                await database.ScriptEvaluateAsync(anonymousClientScript,
                    new RedisKey[] { _clientStoreKey, heartbeatKey, instanceConnectionsKey },
                    new RedisValue[] { client.ConnectionId, client.ToJsonString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds(), _heartbeatTtlSeconds });
            }
        }

        public async Task RefreshHeartbeatAsync(string connectionId)
        {
            var database = GetDatabase();
            await database.KeyExpireAsync(GetHeartbeatKey(connectionId), TimeSpan.FromSeconds(_heartbeatTtlSeconds));
        }

        public async Task<int> RefreshInstanceHeartbeatsAsync(CancellationToken cancellationToken = default)
        {
            var database = GetDatabase();
            var instanceConnectionsKey = GetInstanceConnectionsKey(InstanceId);
            var connectionIds = await database.SetMembersAsync(instanceConnectionsKey);

            var refreshed = 0;
            foreach (var connectionIdValue in connectionIds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var connectionId = connectionIdValue.ToString();
                if (string.IsNullOrWhiteSpace(connectionId))
                {
                    continue;
                }

                var heartbeatKey = GetHeartbeatKey(connectionId);
                var exists = await database.HashExistsAsync(_clientStoreKey, connectionId);
                if (!exists)
                {
                    await database.SetRemoveAsync(instanceConnectionsKey, connectionId);
                    await database.KeyDeleteAsync(heartbeatKey);
                    continue;
                }

                await database.KeyExpireAsync(heartbeatKey, TimeSpan.FromSeconds(_heartbeatTtlSeconds));
                refreshed++;
            }

            return refreshed;
        }

        public async Task<int> CleanupStaleClientsAsync(int scanBatchSize, CancellationToken cancellationToken = default)
        {
            var database = GetDatabase();
            var entries = await database.HashGetAllAsync(_clientStoreKey);
            var removed = 0;
            foreach (var entry in entries.Take(Math.Max(1, scanBatchSize)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var connectionId = entry.Name.ToString();
                if (string.IsNullOrWhiteSpace(connectionId))
                {
                    continue;
                }

                var heartbeatExists = await database.KeyExistsAsync(GetHeartbeatKey(connectionId));
                if (heartbeatExists)
                {
                    continue;
                }

                var (success, _) = await TryRemoveInternalAsync(connectionId);
                if (success)
                {
                    removed++;
                }
            }

            return removed;
        }

        public async Task<int> CleanupCurrentInstanceAsync(CancellationToken cancellationToken = default)
        {
            var database = GetDatabase();
            var instanceConnectionsKey = GetInstanceConnectionsKey(InstanceId);
            var connectionIds = await database.SetMembersAsync(instanceConnectionsKey);
            var removed = 0;

            foreach (var connectionIdValue in connectionIds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var connectionId = connectionIdValue.ToString();
                if (string.IsNullOrWhiteSpace(connectionId))
                {
                    continue;
                }

                var (success, _) = await TryRemoveInternalAsync(connectionId);
                if (success)
                {
                    removed++;
                }
            }

            await database.KeyDeleteAsync(instanceConnectionsKey);
            return removed;
        }

        public async Task<bool> RemoveAsync(string connectionId)
        {
            var (success, _) = await TryRemoveInternalAsync(connectionId);
            return success;
        }

        public async Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction)
        {
            var (success, client) = await TryRemoveInternalAsync(connectionId);
            clientAction?.Invoke(client);
            return success;
        }

        public async Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction)
        {
            var database = GetDatabase();
            var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                clientAction?.Invoke(null);
                return false;
            }

            var onlineClient = JsonSerializer.Deserialize<OnlineClient>(clientValue.ToString());
            clientAction?.Invoke(onlineClient);
            return true;
        }

        public async Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
        {
            var database = GetDatabase();
            var clientsEntries = await database.HashGetAllAsync(_clientStoreKey);
            return clientsEntries
                .Select(entry => JsonSerializer.Deserialize<OnlineClient>(entry.Value))
                .Cast<IOnlineClient>()
                .ToImmutableList();
        }


        public async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier)
        {
            var database = GetDatabase();
            var userConnectionsKey = GetUserConnectionsKey(userIdentifier);

            var connectionIdValues = await database.SetMembersAsync(userConnectionsKey);
            if (connectionIdValues.Length == 0)
            {
                return ImmutableList<IOnlineClient>.Empty;
            }

            var clientValues = await database.HashGetAsync(_clientStoreKey, connectionIdValues);

            return clientValues
                .Where(clientValue => !clientValue.IsNullOrEmpty)
                .Select(clientValue => JsonSerializer.Deserialize<OnlineClient>(clientValue.ToString()))
                .Cast<IOnlineClient>()
                .ToImmutableList();
        }

        private IDatabase GetDatabase()
        {
            return _database.GetDatabase();
        }

        private string GetUserConnectionsKey(UserIdentifier userIdentifier)
        {
            return $"{_userStoreKey}:{userIdentifier.ToUserIdentifierString()}";
        }

        private string GetHeartbeatKey(string connectionId)
        {
            return $"{_heartbeatKeyPrefix}:{connectionId}";
        }

        private string GetInstanceConnectionsKey(string instanceId)
        {
            return $"{_instanceStoreKeyPrefix}:{instanceId}";
        }


        private async Task<(bool Success, IOnlineClient Client)> TryRemoveInternalAsync(string connectionId)
        {
            var database = GetDatabase();

            var clientJson = await database.HashGetAsync(_clientStoreKey, connectionId);
            if (clientJson.IsNullOrEmpty)
            {
                return (false, null);
            }

            var client = JsonSerializer.Deserialize<OnlineClient>(clientJson.ToString());
            var userIdentifier = client.ToUserIdentifierOrNull();
            var instanceId = GetInstanceId(client);

            var userConnectionsKey = userIdentifier != null
                ? GetUserConnectionsKey(userIdentifier)
                : null;

            const string script = @"
            -- KEYS[1] = _clientStoreKey
            -- KEYS[2] = userConnectionsKey
            -- KEYS[3] = heartbeatKey
            -- KEYS[4] = instanceConnectionsKey
            -- ARGV[1] = connectionId

            redis.call('HDEL', KEYS[1], ARGV[1]);
            redis.call('DEL', KEYS[3]);

            if KEYS[2] ~= '' then
                redis.call('SREM', KEYS[2], ARGV[1]);
            end

            if KEYS[4] ~= '' then
                redis.call('SREM', KEYS[4], ARGV[1]);
            end

            return ARGV[2];
            ";

            var keys = new List<RedisKey> { _clientStoreKey };
            if (userConnectionsKey != null)
            {
                keys.Add(userConnectionsKey);
            }
            else
            {
                keys.Add(string.Empty);
            }

            keys.Add(GetHeartbeatKey(connectionId));
            keys.Add(!instanceId.IsNullOrWhiteSpace() ? GetInstanceConnectionsKey(instanceId) : string.Empty);

            await database.ScriptEvaluateAsync(script, keys.ToArray(), new RedisValue[] { connectionId, clientJson });

            return (true, client);
        }

        private static string GetInstanceId(IOnlineClient client)
        {
            if (client?.Properties == null)
            {
                return null;
            }

            if (!client.Properties.TryGetValue("instanceId", out var instanceIdObj) || instanceIdObj == null)
            {
                return null;
            }

            if (instanceIdObj is string instanceIdString)
            {
                return instanceIdString;
            }

            if (instanceIdObj is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
            {
                return jsonElement.GetString();
            }

            return instanceIdObj.ToString();
        }
    }
}
