using Castle.Core.Logging;
using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
namespace ADTO.DCloud.Tasks.TaskManage
{
    public abstract class BaseCycleConfigParser : ICycleConfigParser
    {
        protected readonly Microsoft.Extensions.Logging.ILogger Logger;

        protected BaseCycleConfigParser(Microsoft.Extensions.Logging.ILogger logger)
        {
            Logger = logger;
        }

        public abstract bool CanParse(string cycleType);
        public abstract TimeSpan GetInterval(string cycleJsonValue);
        public abstract DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime);

        protected T DeserializeConfig<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, $"Failed to deserialize cycle config: {json}");
                throw new ArgumentException("Invalid cycle configuration", ex);
            }
        }
    }
}