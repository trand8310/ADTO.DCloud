using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace ADTO.DCloud.EntityFrameworkCore;

public static class DCloudDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<DCloudDbContext> builder, string connectionString)
    {
        //builder.UseSqlServer(connectionString);

        builder.UseSqlServer(connectionString,x => x.UseCompatibilityLevel(120));
        //builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    public static void Configure(DbContextOptionsBuilder<DCloudDbContext> builder, DbConnection connection)
    {
        //builder.UseSqlServer(connection);
        builder.UseSqlServer(connection, x => x.UseCompatibilityLevel(120));
        //builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}

