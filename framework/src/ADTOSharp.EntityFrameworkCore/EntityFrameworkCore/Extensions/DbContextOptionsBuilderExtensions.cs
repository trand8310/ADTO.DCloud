using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ADTOSharp.EntityFrameworkCore.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder AddADTOSharpDbContextOptionsExtension(this DbContextOptionsBuilder optionsBuilder)
    {
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new ADTOSharpDbContextOptionsExtension());
        return optionsBuilder;
    }

    public static DbContextOptionsBuilder<TContext> AddADTOSharpDbContextOptionsExtension<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder)
        where TContext : DbContext
    {
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new ADTOSharpDbContextOptionsExtension());
        return optionsBuilder;
    }
}