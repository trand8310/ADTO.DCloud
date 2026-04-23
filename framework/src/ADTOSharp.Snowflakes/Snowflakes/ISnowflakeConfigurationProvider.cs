using JetBrains.Annotations;

namespace ADTOSharp.Snowflakes
{
    public interface ISnowflakeConfigurationProvider
    {
        SnowflakeConfiguration Get([NotNull] string name);
    }
}
