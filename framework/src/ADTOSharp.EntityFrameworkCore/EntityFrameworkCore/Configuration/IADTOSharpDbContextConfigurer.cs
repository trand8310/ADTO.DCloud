using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore.Configuration;

public interface IADTOSharpDbContextConfigurer<TDbContext>
    where TDbContext : DbContext
{
    void Configure(ADTOSharpDbContextConfiguration<TDbContext> configuration);
}