using System;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore.Configuration;

public class ADTOSharpDbContextConfigurerAction<TDbContext> : IADTOSharpDbContextConfigurer<TDbContext>
    where TDbContext : DbContext
{
    public Action<ADTOSharpDbContextConfiguration<TDbContext>> Action { get; set; }

    public ADTOSharpDbContextConfigurerAction(Action<ADTOSharpDbContextConfiguration<TDbContext>> action)
    {
        Action = action;
    }

    public void Configure(ADTOSharpDbContextConfiguration<TDbContext> configuration)
    {
        Action(configuration);
    }
}