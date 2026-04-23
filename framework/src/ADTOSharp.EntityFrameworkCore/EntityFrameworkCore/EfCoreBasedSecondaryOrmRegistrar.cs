using System;

using ADTOSharp.EntityFramework;

namespace ADTOSharp.EntityFrameworkCore;

public class EfCoreBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
{
    public EfCoreBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
        : base(dbContextType, dbContextEntityFinder)
    {
    }

    public override string OrmContextKey { get; } = ADTOSharpConsts.Orms.EntityFrameworkCore;
}