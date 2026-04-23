using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataIcons.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Tasks.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.Threading.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Transactions;
using static ADTOSharp.SequentialGuidGenerator;

namespace ADTO.DCloud.Tests;

/// <summary>
/// 测试方法,供测试使用
/// </summary>
[AllowAnonymous]
public class TestAppService : DCloudAppServiceBase, ITestAppService
{
    #region Fields
    // public IPermissionManager PermissionManager { protected get; set; }
    private readonly IDapperSqlExecutor _sqlExecutor;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<ApplicationLanguageText, Guid> _languagTextRepository;
    #endregion

    #region Ctor
    public TestAppService(IDapperSqlExecutor sqlExecutor, IUnitOfWorkManager unitOfWorkManager, IRepository<ApplicationLanguageText, Guid> languagTextRepository)
    {
        _sqlExecutor = sqlExecutor;
        _unitOfWorkManager = unitOfWorkManager;
        _languagTextRepository = languagTextRepository;

    }
    #endregion

    #region Methods

    public Guid NewGuid()
    {
        return IocManager.Instance.Resolve<IGuidGenerator>().Create();
    }

    private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

    public static Guid CreateSqlSequentialGuid(DateTime time)
    {
        // 1. 随机部分（10字节）
        byte[] randomBytes = new byte[10];
        RandomNumberGenerator.Fill(randomBytes);

        // 2. SQL Server 推荐：使用毫秒时间戳
        long millis = new DateTimeOffset(time).ToUnixTimeMilliseconds();

        // 取后 6 字节（够用 8900 年）
        byte[] timeBytes = BitConverter.GetBytes(millis);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(timeBytes);

        byte[] guidBytes = new byte[16];

        // 前 10 字节随机
        Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);

        // 后 6 字节时间戳（决定排序）
        Buffer.BlockCopy(timeBytes, 2, guidBytes, 10, 6);

        return new Guid(guidBytes);
    }




    public async Task<bool> UpdateLanguagText()
    {
        var guid = IocManager.Instance.Resolve<IGuidGenerator>();
        var query = _languagTextRepository.GetAll().AsNoTracking().OrderBy(o => o.CreationTime);
        var list = await query.ToListAsync();

        var dt = System.DateTime.UtcNow;
        var dt2 = System.DateTime.Now;

        foreach (var item in list.OrderBy(o => o.CreationTime))
        {
            await _sqlExecutor.ExecuteAsync("UPDATE LanguageTexts SET id = @nid where id = @id ", new { id = item.Id, nid = CreateSqlSequentialGuid(item.CreationTime) });


            //指定字段修改
            //await this._languagTextRepository.UpdateAsync(id, async entity =>
            //{
            //    entity.Id = guid.Create();
            //});
        }
        return true;



        //return IocManager.Instance.Resolve<IGuidGenerator>().Create();
    }


    public async Task CreateNewPermission()
    {

        //var context = IocManager.Instance.Resolve<IPermissionDefinitionContext>();
        // contenxt.GetPermissionOrNull("");
        // var root = context.GetPermissionOrNull(PermissionNames.Pages);
        // PermissionFinder.GetAllPermissions();
        var context = PermissionManager as PermissionDefinitionContextBase;
        var all1 = PermissionManager.GetAllPermissions();
       // var pages = PermissionManager.GetPermissionOrNull(PermissionNames.Pages);
        context.CreatePermission(PermissionNames.Pages_DemoUiComponents, new LocalizableString("DemoUiComponents", DCloudConsts.LocalizationSourceName));
        var all2 = PermissionManager.GetAllPermissions();
        
        //var root = PermissionManager.GetPermissionOrNull(PermissionNames.Pages);

        await Task.CompletedTask;
        //PermissionManager.
    }
    ///// <summary>
    ///// 读写分离演示
    ///// </summary>
    ///// <returns></returns>
    //public Task<List<string>> ReadWriteSeparate()
    //{
    //    //切换到从库,适合只读
    //    return _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
    //    {
    //        _unitOfWorkManager.Current.SetReadOnly(true);
    //        var query = await _moduleRepository.GetAllReadonlyAsync();
    //        var list = await query.Select(s => s.ModuleName).ToListAsync();
    //        return list;
    //    }, new UnitOfWorkOptions() { IsTransactional = false, IsolationLevel = IsolationLevel.ReadCommitted, Scope = TransactionScopeOption.Suppress });
    //}



    //public Task<List<string>> SwitchWRAsync()
    //{
    //    //切换到从库,适合只读

    //    //return _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
    //    //{
    //    //    _unitOfWorkManager.Current.SetReadOnly(true);
    //    //    var query = await _moduleRepository.GetAllReadonlyAsync();
    //    //    var list = await query.Select(s => s.ModuleName).ToListAsync();
    //    //    return list;
    //    //}, new UnitOfWorkOptions() { IsTransactional = false, IsolationLevel = IsolationLevel.ReadCommitted, Scope = TransactionScopeOption.Suppress });
    //}

    //private static ILocalizableString L(string name)
    //{
    //    return new LocalizableString(name, DCloudConsts.LocalizationSourceName);
    //}
    #endregion
}

