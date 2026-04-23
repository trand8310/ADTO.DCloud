using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.EntityFrameworkCore.Configuration;

public class ADTOSharpDbContextConfiguration<TDbContext>
    where TDbContext : DbContext
{
    public string ConnectionString { get; internal set; }

    public DbConnection ExistingConnection { get; internal set; }

    public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }

    public ADTOSharpDbContextConfiguration(string connectionString, DbConnection existingConnection)
    {
        ConnectionString = connectionString;
        ExistingConnection = existingConnection;

        DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
    }
}