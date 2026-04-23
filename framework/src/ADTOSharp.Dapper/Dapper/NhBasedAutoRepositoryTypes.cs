using ADTOSharp.Dapper.Repositories;

namespace ADTOSharp.Dapper
{
    public class NhBasedDapperAutoRepositoryTypes
    {
        static NhBasedDapperAutoRepositoryTypes()
        {
            Default = new DapperAutoRepositoryTypeAttribute(
                typeof(IDapperRepository<>),
                typeof(IDapperRepository<,>),
                typeof(DapperRepositoryBase<>),
                typeof(DapperRepositoryBase<,>)
            );
        }

        public static DapperAutoRepositoryTypeAttribute Default { get; }
    }
}
