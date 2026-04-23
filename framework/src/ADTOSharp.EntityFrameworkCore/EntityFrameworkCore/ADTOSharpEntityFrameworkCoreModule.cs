using System;
using System.Reflection;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.EntityFramework;
using ADTOSharp.EntityFramework.Repositories;
using ADTOSharp.EntityFrameworkCore.Configuration;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.EntityFrameworkCore.Uow;
using ADTOSharp.Modules;
using ADTOSharp.Orm;
using ADTOSharp.Reflection;
using ADTOSharp.Reflection.Extensions;
using Castle.MicroKernel.Registration;

namespace ADTOSharp.EntityFrameworkCore;

/// <summary>
/// This module is used to implement "Data Access Layer" in EntityFramework.
/// </summary>
[DependsOn(typeof(ADTOSharpEntityFrameworkCommonModule))]
public class ADTOSharpEntityFrameworkCoreModule : ADTOSharpModule
{
    private readonly ITypeFinder _typeFinder;

    public ADTOSharpEntityFrameworkCoreModule(ITypeFinder typeFinder)
    {
        _typeFinder = typeFinder;
    }

    public override void PreInitialize()
    {
        IocManager.Register<IADTOSharpEfCoreConfiguration, ADTOSharpEfCoreConfiguration>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpEntityFrameworkCoreModule).GetAssembly());

        IocManager.IocContainer.Register(
            Component.For(typeof(IDbContextProvider<>))
                .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                .LifestyleTransient()
            );

        RegisterGenericRepositoriesAndMatchDbContexes();
    }

    private void RegisterGenericRepositoriesAndMatchDbContexes()
    {
        var dbContextTypes =
            _typeFinder.Find(type =>
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsPublic &&
                       !typeInfo.IsAbstract &&
                       typeInfo.IsClass &&
                       typeof(ADTOSharpDbContext).IsAssignableFrom(type);
            });

        if (dbContextTypes.IsNullOrEmpty())
        {
            Logger.Warn("No class found derived from ADTOSharpDbContext.");
            return;
        }

        using (IScopedIocResolver scope = IocManager.CreateScope())
        {
            foreach (var dbContextType in dbContextTypes)
            {
                Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);

                scope.Resolve<IEfGenericRepositoryRegistrar>().RegisterForDbContext(dbContextType, IocManager, EfCoreAutoRepositoryTypes.Default);

                IocManager.IocContainer.Register(
                    Component.For<ISecondaryOrmRegistrar>()
                        .Named(Guid.NewGuid().ToString("N"))
                        .Instance(new EfCoreBasedSecondaryOrmRegistrar(dbContextType, scope.Resolve<IDbContextEntityFinder>()))
                        .LifestyleTransient()
                );
            }

            scope.Resolve<IDbContextTypeMatcher>().Populate(dbContextTypes);
        }
    }
}