using ADTOSharp.Dependency;
using ADTOSharp.Modules;
using ADTOSharp.Orm;
using ADTOSharp.Reflection.Extensions;
using Slapper;

namespace ADTOSharp.Dapper
{
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpDapperModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactionScopeAvailable = false;
            Slapper.AutoMapper.Configuration.TypeConverters.Add(new AutoMapper.Configuration.EnumConverter());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpDapperModule).GetAssembly());

            using (IScopedIocResolver scope = IocManager.CreateScope())
            {
                ISecondaryOrmRegistrar[] additionalOrmRegistrars = scope.ResolveAll<ISecondaryOrmRegistrar>();

                foreach (ISecondaryOrmRegistrar registrar in additionalOrmRegistrars)
                {
                    if (registrar.OrmContextKey == ADTOSharpConsts.Orms.EntityFramework)
                    {
                        registrar.RegisterRepositories(IocManager, EfBasedDapperAutoRepositoryTypes.Default);
                    }

                    if (registrar.OrmContextKey == ADTOSharpConsts.Orms.NHibernate)
                    {
                        registrar.RegisterRepositories(IocManager, NhBasedDapperAutoRepositoryTypes.Default);
                    }

                    if (registrar.OrmContextKey == ADTOSharpConsts.Orms.EntityFrameworkCore)
                    {
                        registrar.RegisterRepositories(IocManager, EfBasedDapperAutoRepositoryTypes.Default);
                    }
                }
            }
        }
    }
}
