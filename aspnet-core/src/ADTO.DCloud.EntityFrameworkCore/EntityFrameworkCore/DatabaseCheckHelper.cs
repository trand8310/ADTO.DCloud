using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.Extensions;
using Microsoft.EntityFrameworkCore;


namespace ADTO.DCloud.EntityFrameworkCore;

/// <summary>
/// 数据库连接性检测工具方法
/// </summary>
public class DatabaseCheckHelper : ITransientDependency
{
    private readonly IDbContextProvider<DCloudDbContext> _dbContextProvider;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public DatabaseCheckHelper(
        IDbContextProvider<DCloudDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager
    )
    {
        _dbContextProvider = dbContextProvider;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public bool Exist(string connectionString)
    {
        if (connectionString.IsNullOrEmpty())
        {
            //connectionString is null for unit tests
            return true;
        }

        try
        {
            using (var uow =_unitOfWorkManager.Begin())
            {
                // Switching to host is necessary for single tenant mode.
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    _dbContextProvider.GetDbContext().Database.OpenConnection();
                    uow.Complete();
                }
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}

