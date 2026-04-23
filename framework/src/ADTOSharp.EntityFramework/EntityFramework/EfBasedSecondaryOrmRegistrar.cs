using System;

namespace ADTOSharp.EntityFramework
{
    public class EfBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
    {
        public EfBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey => ADTOSharpConsts.Orms.EntityFramework;
    }
}
