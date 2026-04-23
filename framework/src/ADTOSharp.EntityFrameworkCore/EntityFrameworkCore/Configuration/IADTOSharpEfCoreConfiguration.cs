using System;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore.Configuration;

public interface IADTOSharpEfCoreConfiguration
{
    public bool UseADTOSharpQueryCompiler { get; set; }

    void AddDbContext<TDbContext>(Action<ADTOSharpDbContextConfiguration<TDbContext>> action)
        where TDbContext : DbContext;
}

public class NullADTOSharpEfCoreConfiguration : IADTOSharpEfCoreConfiguration
{
    /// <summary>
    /// Gets single instance of <see cref="NullADTOSharpEfCoreConfiguration"/> class.
    /// </summary>
    public static NullADTOSharpEfCoreConfiguration Instance { get; } = new NullADTOSharpEfCoreConfiguration();

    public bool UseADTOSharpQueryCompiler { get; set; }

    public void AddDbContext<TDbContext>(Action<ADTOSharpDbContextConfiguration<TDbContext>> action) where TDbContext : DbContext
    {

    }
}