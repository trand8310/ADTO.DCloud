using ADTO.DCloud.EntityFrameworkCore.Seed.Host;
using ADTO.DCloud.EntityFrameworkCore.Seed.Tenants;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore.Uow;
using ADTOSharp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Transactions;


namespace ADTO.DCloud.EntityFrameworkCore.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            Action<DCloudDbContext> seedAction = (context) => SeedHostDb(context);
            WithDbContext<DCloudDbContext>(iocResolver, seedAction);
        }

        public static void SeedHostDb(DCloudDbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new InitialHostDbBuilder(context).Create();

            // Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();
            new TenantRoleAndUserBuilder(context, Guid.Parse("00000000-0000-0000-0000-000000000001")).Create();
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {

                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);
                    contextAction(context);
                    //var _openIddictScopeRepository = iocResolver.Resolve<IOpenIddictScopeRepository>();
                    //var _scopeManager = iocResolver.Resolve<IOpenIddictScopeManager>();

                    //var scope = _openIddictScopeRepository.FirstOrDefault(p => p.Name == "DCloud");
                    //if (scope == null)
                    //{

                    //    _ = _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                    //    {
                    //        Name = "DCloud",
                    //        DisplayName = "DCloud API",
                    //        Resources = { "DCloud" }
                    //    });
                    //}



                    //var scope = dbContext.Scopes.IgnoreQueryFilters()
                    //                           .FirstOrDefault(ef => ef.Name == "DCloud");

                    //if (scope == null)
                    //{
                    //    dbContext.Scopes.Add(new OpenIddictScope
                    //    {

                    //        Name = "DCloud",
                    //        DisplayName = "DCloud API",
                    //        Resources = "DCloud",
                    //    });
                    //    dbContext.SaveChanges();
                    //}


                    uow.Complete();
                }


                using (var uow = uowManager.Object.Begin(new UnitOfWorkOptions { IsTransactional = false, Scope = TransactionScopeOption.RequiresNew }))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);
                    using (var connection = context.Database.GetDbConnection())
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            foreach (var entityType in context.Model.GetEntityTypes())
                            {
                                var clrType = entityType.ClrType;
                                if (clrType == null) continue;
                                var tableName = entityType.GetTableName();
                                var classDescription = clrType.GetCustomAttribute<DescriptionAttribute>()?.Description;
                                if (!string.IsNullOrEmpty(classDescription))
                                {
                                    command.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.extended_properties WHERE name = 'MS_Description' AND major_id = OBJECT_ID('dbo." + tableName + "') and minor_id = 0) EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'" + classDescription + "', @level0type = N'Schema', @level0name = N'dbo', @level1type = N'Table', @level1name = N'" + entityType.GetTableName() + "'";
                                    command.ExecuteNonQuery();
                                }

                                //获取属性描述
                                foreach (var property in entityType.GetProperties())
                                {
                                    var columnName = property.GetColumnName();
                                    var propertyInfo = clrType.GetProperty(property.Name);
                                    if (propertyInfo == null) continue;
                                    var propertyDescription = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                                    if (!string.IsNullOrEmpty(propertyDescription))
                                    {
                                        command.CommandText = @"IF NOT EXISTS (SELECT * FROM sys.extended_properties WHERE name = 'MS_Description' AND major_id = OBJECT_ID('dbo." + tableName + "') and  minor_id = COLUMNPROPERTY(OBJECT_ID('dbo." + tableName + "'), '" + columnName + "', 'ColumnId'))  EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'" + propertyDescription + "', @level0type = N'Schema', @level0name = N'dbo', @level1type = N'Table', @level1name = N'" + tableName + "',@level2type = N'COLUMN',@level2name = N'" + columnName + "' ";
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    uow.Complete();


                }
            }
        }
    }
}

