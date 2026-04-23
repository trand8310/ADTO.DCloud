using System;
using ADTOSharp.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ADTOSharp.EntityFrameworkCore;

public static class ADTOSharpEfCoreServiceCollectionExtensions
{
    public static void AddADTOSharpDbContext<TDbContext>(
        this IServiceCollection services,
        Action<ADTOSharpDbContextConfiguration<TDbContext>> action)
        where TDbContext : DbContext
    {
        services.AddSingleton(
            typeof(IADTOSharpDbContextConfigurer<TDbContext>),
            new ADTOSharpDbContextConfigurerAction<TDbContext>(action)
        );
    }
}