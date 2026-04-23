using ADTOSharp.Extensions;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ADTOSharp.Dependency;


namespace ADTOSharp.Snowflakes
{
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpSnowflakesModule : ADTOSharpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpSnowflakesModule).GetAssembly());
        }
        public override void PostInitialize()
        {
            IocManager.Using<IOptions<ADTOSharpSnowflakesOptions>>(optionsAccessor =>
            {
                var configuration = IocManager.Resolve<IConfiguration>();
                optionsAccessor.Value.Configure(configuration);
                optionsAccessor.Value.Snowflakes.ConfigureDefault(c =>
                {
                    c.WorkerId = 1L;
                    c.DatacenterId = 1L;
                });
            });
        }
    }
}
