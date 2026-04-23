using System.Configuration;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Extensions;

namespace ADTOSharp.Runtime.Caching.Redis
{
    public class ADTOSharpRedisCacheOptions
    {
        public IADTOSharpStartupConfiguration ADTOSharpStartupConfiguration { get; }

        private const string ConnectionStringKey = "ADTOSharp.Redis.Cache";

        private const string DatabaseIdSettingKey = "ADTOSharp.Redis.Cache.DatabaseId";

        public string ConnectionString { get; set; }

        public int DatabaseId { get; set; }

        public string OnlineClientsStoreKey = "ADTOSharp.RealTime.OnlineClients";

        public string KeyPrefix { get; set; }

        public bool TenantKeyEnabled { get; set; }

        /// <summary>
        /// Required for serialization
        /// </summary>
        public ADTOSharpRedisCacheOptions()
        {
            
        }
        
        public ADTOSharpRedisCacheOptions(IADTOSharpStartupConfiguration startupConfiguration)
        {
            ADTOSharpStartupConfiguration = startupConfiguration;

            ConnectionString = GetDefaultConnectionString();
            DatabaseId = GetDefaultDatabaseId();
            KeyPrefix = "";
            TenantKeyEnabled = false;
        }

        private static int GetDefaultDatabaseId()
        {
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
            if (appSetting.IsNullOrEmpty())
            {
                return -1;
            }

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId))
            {
                return -1;
            }

            return databaseId;
        }

        private static string GetDefaultConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace())
            {
                return "localhost";
            }

            return connStr.ConnectionString;
        }

        
    }
}