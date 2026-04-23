
using ADTOSharp.Dependency;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace ADTOSharp.Snowflakes
{
    public class DefaultSnowflakeConfigurationProvider : ISnowflakeConfigurationProvider, ITransientDependency
    {
        protected ADTOSharpSnowflakesOptions Options { get; }
        public DefaultSnowflakeConfigurationProvider(IOptions<ADTOSharpSnowflakesOptions> options)
        {
            Options = options.Value;
        }

        [NotNull]
        public virtual SnowflakeConfiguration Get([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return Options.Snowflakes.GetConfiguration(name);
        }
    }
}
