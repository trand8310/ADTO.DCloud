using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ADTOSharp.EntityFrameworkCore;

public class ADTOSharpDbContextOptionsExtension : IDbContextOptionsExtension
{
    public void ApplyServices(IServiceCollection services)
    {
        var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ICompiledQueryCacheKeyGenerator));
        if (serviceDescriptor != null && serviceDescriptor.ImplementationType != null)
        {
            services.Remove(serviceDescriptor);
            services.AddScoped(serviceDescriptor.ImplementationType);
            services.Add(ServiceDescriptor.Scoped<ICompiledQueryCacheKeyGenerator>(provider =>
                ActivatorUtilities.CreateInstance<ADTOSharpCompiledQueryCacheKeyGenerator>(provider,
                    provider.GetRequiredService(serviceDescriptor.ImplementationType)
                        .As<ICompiledQueryCacheKeyGenerator>())));
        }

        services.Replace(ServiceDescriptor.Scoped<IAsyncQueryProvider, ADTOSharpEntityQueryProvider>());
        services.AddSingleton<ADTOSharpEfCoreCurrentDbContext>();
    }

    public void Validate(IDbContextOptions options)
    {
    }

    public DbContextOptionsExtensionInfo Info => new ADTOSharpOptionsExtensionInfo(this);

    private class ADTOSharpOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public ADTOSharpOptionsExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        public override bool IsDatabaseProvider => false;

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ADTOSharpOptionsExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }

        public override string LogFragment => "ADTOSharpOptionsExtension";
    }
}