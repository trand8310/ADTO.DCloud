using ADTO.DCloud.Configuration;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;


namespace ADTO.DCloud.EntityFrameworkCore;

/// <summary>
/// 实现在UNITWORK工作模式下,动态解析数据库连接,实现动态的读写分离
/// </summary>
public class ReadWriteConnectionStringResolver : DefaultConnectionStringResolver
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IADTOSharpStartupConfiguration _configuration;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ReadWriteConnectionStringResolver(
        IWebHostEnvironment env,
        IADTOSharpStartupConfiguration configuration,
        IUnitOfWorkManager unitOfWorkManager)
        : base(configuration)
    {
        _hostingEnvironment = env;
        _configuration = configuration;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public override string GetNameOrConnectionString(ConnectionStringResolveArgs args)
    {
        var currentUow = _unitOfWorkManager.Current;
        if (currentUow != null && currentUow.IsReadOnly())
        {
            var configuration = _hostingEnvironment.GetAppConfiguration();
            var readOnlyConnectString = configuration.GetConnectionString(DCloudConsts.ReadOnlyNameOrConnectionString);
            if(!readOnlyConnectString.IsNullOrWhiteSpace())
            {
                return readOnlyConnectString;
            }
        }
        return base.GetNameOrConnectionString(args);
    }

    //private string GetConnectionStringName(ConnectionStringResolveArgs args)
    //{
    //    if (args["DbContextConcreteType"] as Type == typeof(AttendanceDbContext))
    //    {
    //        return OAConsts.ConnectionAttendanceName;
    //    }
    //    return OAConsts.ConnectionStringName;
    //}

    //public string GetNameOrConnectionString(ConnectionStringResolveArgs args)
    //{
    //    var currentUow = _unitOfWorkManager.Current;
    //    if (currentUow != null && currentUow.IsReadOnly())
    //    {
    //        if (System.Configuration.ConfigurationManager.ConnectionStrings[DCloudConsts.ReadOnlyNameOrConnectionString] != null)
    //        {
    //            return DCloudConsts.ReadOnlyNameOrConnectionString;
    //        }
    //    }
    //    return DCloudConsts.ConnectionStringName;
    //}
    //public Task<string> GetNameOrConnectionStringAsync(ConnectionStringResolveArgs args)
    //{
    //    return Task.FromResult(GetNameOrConnectionString(args));
    //}



    //public string GetNameOrConnectionString(ConnectionStringResolveArgs args)
    //{
    //    var connectStringName = this.GetConnectionStringName(args);
    //    if (connectStringName != null)
    //    {
    //        var configuration = AppConfigurations.Get(_hostingEnvironment.ContentRootPath, _hostingEnvironment.EnvironmentName, _hostingEnvironment.IsDevelopment());

    //        var connectString = configuration.GetConnectionString(connectStringName);
    //        return DESHelper.Decrypt(connectString);
    //    }





    //    var defaultConnectionString = _configuration.GetConnectionString("Default") ?? throw new Exception("Missing Default connection");
    //    var currentUow = _unitOfWorkManager.Current;
    //    if (currentUow != null && currentUow.IsReadOnly())
    //    {
    //        var readConnectionString = _configuration.GetConnectionString("ReadOnly");
    //        if (!readConnectionString.IsNullOrWhiteSpace())
    //        {
    //            return readConnectionString;
    //        }
    //    }
    //    return defaultConnectionString;

    //}
    //public Task<string> GetNameOrConnectionStringAsync(ConnectionStringResolveArgs args)
    //{
    //    return Task.FromResult(GetNameOrConnectionString(args));
    //}


    //private string GetConnectionStringName(ConnectionStringResolveArgs args)
    //{
    //    if (args["DbContextConcreteType"] as Type == typeof(AttendanceDbContext))
    //    {
    //        return OAConsts.ConnectionAttendanceName;
    //    }
    //    return OAConsts.ConnectionStringName;
    //}



    //public string Resolve(string? connectionStringName = null)
    //{
    //    var defaultConnectionString = _configuration.GetConnectionString("Default") ?? throw new Exception("Missing Default connection");
    //    var currentUow = _unitOfWorkManager.Current;
    //    if (currentUow != null && currentUow.IsReadOnly())
    //    {
    //        var readConnectionString = _configuration.GetConnectionString("ReadOnly");
    //        if (!readConnectionString.IsNullOrWhiteSpace())
    //        {
    //            return readConnectionString;
    //        }
    //    }
    //    return defaultConnectionString;
    //}

    //public Task<string> ResolveAsync(string? connectionStringName = null)
    //{
    //    return Task.FromResult(Resolve(connectionStringName));
    //}
}

