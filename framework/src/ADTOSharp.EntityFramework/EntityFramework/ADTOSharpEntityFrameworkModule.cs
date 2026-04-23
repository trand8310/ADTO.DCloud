using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFramework.Interceptors;
using ADTOSharp.EntityFramework.Repositories;
using ADTOSharp.EntityFramework.Uow;
using ADTOSharp.Modules;
using ADTOSharp.Orm;
using ADTOSharp.Reflection;

using Castle.MicroKernel.Registration;

namespace ADTOSharp.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    [DependsOn(typeof(ADTOSharpEntityFrameworkCommonModule))]
    public class ADTOSharpEntityFrameworkModule : ADTOSharpModule
    {
        private static WithNoLockInterceptor _withNoLockInterceptor;
        private static readonly object WithNoLockInterceptorSyncObj = new object();

        private readonly ITypeFinder _typeFinder;

        public ADTOSharpEntityFrameworkModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            Configuration.ReplaceService<IUnitOfWorkFilterExecuter>(() =>
            {
                IocManager.IocContainer.Register(
                    Component
                    .For<IUnitOfWorkFilterExecuter, IEfUnitOfWorkFilterExecuter>()
                    .ImplementedBy<EfDynamicFiltersUnitOfWorkFilterExecuter>()
                    .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            if (!Configuration.UnitOfWork.IsTransactionScopeAvailable)
            {
                IocManager.RegisterIfNot<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);
            }

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
                );

            RegisterGenericRepositoriesAndMatchDbContexes();
            RegisterWithNoLockInterceptor();
        }

        private void RegisterWithNoLockInterceptor()
        {
            lock (WithNoLockInterceptorSyncObj)
            {
                if (_withNoLockInterceptor != null)
                {
                    return;
                }

                _withNoLockInterceptor = IocManager.Resolve<WithNoLockInterceptor>();
                DbInterception.Add(_withNoLockInterceptor);
            }
        }

        private void RegisterGenericRepositoriesAndMatchDbContexes()
        {
            var dbContextTypes =
                _typeFinder.Find(type =>
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(ADTOSharpDbContext).IsAssignableFrom(type)
                    );

            if (dbContextTypes.IsNullOrEmpty())
            {
                Logger.Warn("No class found derived from ADTOSharpDbContext.");
                return;
            }

            using (var scope = IocManager.CreateScope())
            {
                var repositoryRegistrar = scope.Resolve<IEfGenericRepositoryRegistrar>();

                foreach (var dbContextType in dbContextTypes)
                {
                    Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);
                    repositoryRegistrar.RegisterForDbContext(dbContextType, IocManager, EfAutoRepositoryTypes.Default);

                    IocManager.IocContainer.Register(
                        Component.For<ISecondaryOrmRegistrar>()
                            .Named(Guid.NewGuid().ToString("N"))
                            .Instance(new EfBasedSecondaryOrmRegistrar(dbContextType, scope.Resolve<IDbContextEntityFinder>()))
                            .LifestyleTransient()
                    );
                }
                
                scope.Resolve<IDbContextTypeMatcher>().Populate(dbContextTypes);
            }
        }
    }
}
