using System;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;

namespace ADTOSharp.EntityFramework.Repositories
{
    public interface IEfGenericRepositoryRegistrar
    {
        void RegisterForDbContext(
            Type dbContextType,
            IIocManager iocManager,
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute
        );

        void RegisterForEntity(
            Type dbContextType,
            Type entityType,
            IIocManager iocManager,
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute
        );
    }
}
