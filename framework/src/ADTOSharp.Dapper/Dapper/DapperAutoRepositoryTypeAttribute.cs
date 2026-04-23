using System;

using ADTOSharp.Domain.Repositories;

using JetBrains.Annotations;

namespace ADTOSharp.Dapper
{
    public class DapperAutoRepositoryTypeAttribute : AutoRepositoryTypesAttribute
    {
        public DapperAutoRepositoryTypeAttribute(
            [NotNull] Type repositoryInterface,
            [NotNull] Type repositoryInterfaceWithPrimaryKey,
            [NotNull] Type repositoryImplementation,
            [NotNull] Type repositoryImplementationWithPrimaryKey)
            : base(repositoryInterface, repositoryInterfaceWithPrimaryKey, repositoryImplementation, repositoryImplementationWithPrimaryKey)
        {
        }
    }
}
