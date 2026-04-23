using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;

namespace ADTOSharp.Orm
{
    public interface ISecondaryOrmRegistrar
    {
        string OrmContextKey { get; }

        void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes);
    }
}
