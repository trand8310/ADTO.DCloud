using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EntityFrameworkCore
{
    //public class ReadWriteDbContextProvider : IDbContextProvider<DCloudDbContext>
    //{
    //    private readonly IServiceProvider _serviceProvider;
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public ReadWriteDbContextProvider(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
    //    {
    //        _serviceProvider = serviceProvider;
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public Task<DCloudDbContext> GetDbContextAsync()
    //    {
    //        return GetDbContextAsync(null);
    //    }

    //    public Task<DCloudDbContext> GetDbContextAsync(MultiTenancySides? multiTenancySide)
    //    {
    //        //return _currentUnitOfWorkProvider.Current.GetDbContextAsync<DCloudDbContext>(multiTenancySide);
    //    }

    //    public DCloudDbContext GetDbContext()
    //    {
    //        return GetDbContext(null);
    //    }

    //    public DCloudDbContext GetDbContext(MultiTenancySides? multiTenancySide)
    //    {
 
    //    }
    //}
}
