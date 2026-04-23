using ADTO.DCloud.DataBase.Model;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.UI;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using static ADTOSharp.Domain.Uow.ADTOSharpDataFilters;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ADTO.DCloud.DataBase
{
    /// <summary>
    /// 数据库相关帮助类
    /// </summary>
    public class DataBaseService : IDataBaseService, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public DataBaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="whereText"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public virtual async Task<IEnumerable<string>> GetSqlColName(string sqlText, string whereText = null)
        {
            StringBuilder sqlbuf = new StringBuilder(sqlText);
            if (!string.IsNullOrWhiteSpace(whereText))
            {
                if (sqlText.Contains("WHERE"))
                    sqlbuf.Append($" AND {whereText}");
                else
                    sqlbuf.Append($" WHERE {whereText}");
            }
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlbuf.ToString();

                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        return Enumerable.Range(0, reader.FieldCount)
                                .Select(i => reader.GetName(i).ToLower())
                                .ToList();
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql 语句,查询表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        //[UnitOfWork(isTransactional: false, scope: TransactionScopeOption.Suppress)]
        [UnitOfWork(false)]
        public virtual async Task<DataTable> FindTable(string sql, object sqlParams = null)
        {
            var dataTable = new DataTable();

            using var scope = _serviceProvider.CreateScope();

            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            var dbContextProvider =
                scope.ServiceProvider.GetRequiredService<IDbContextProvider<DCloudDbContext>>();

            using var uow = uowManager.Begin(TransactionScopeOption.RequiresNew);

            var context = await dbContextProvider.GetDbContextAsync();
            var connection = context.Database.GetDbConnection();

            using var command = connection.CreateCommand();
            command.CommandText = SetSql(sql);

            var trans = context.Database.CurrentTransaction;
            if (trans != null)
            {
                command.Transaction = trans.GetDbTransaction();
            }

            if (sqlParams is IDictionary dict)
            {
                foreach (DictionaryEntry param in dict)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Key.ToString();
                    dbParameter.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParameter);
                }
            }
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            await uow.CompleteAsync();

            return dataTable;


        }

        /// <summary>
        /// 执行sql 语句,查询表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="whereStr"></param>
        /// <param name="orderSql"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public virtual async Task<DataTable> FindTable(string sql, string whereStr, string orderSql, object sqlParams = null)
        {
            StringBuilder sqlbuf = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(whereStr))
            {
                if (sql.ToUpper().Contains("WHERE"))
                    sqlbuf.Append($" AND {whereStr}");
                else
                    sqlbuf.Append($" WHERE {whereStr}");
            }
            if (!string.IsNullOrWhiteSpace(orderSql))
            {
                sqlbuf.Append($" {orderSql}");
            }

            var dataTable = new DataTable();
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {

                    command.CommandText = sqlbuf.ToString();
                    if (sqlParams != null)
                    {
                        var dictionary = sqlParams as IDictionary;
                        if (dictionary != null)
                        {
                            foreach (DictionaryEntry param in dictionary)
                            {
                                var dbParameter = command.CreateParameter();
                                dbParameter.ParameterName = param.Key.ToString();
                                dbParameter.Value = param.Value;
                                command.Parameters.Add(dbParameter);
                            }
                        }
                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                    // 显式关闭连接
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// 执行sql 语句,查询表分页数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <param name="pagination"></param>
        /// <param name="whereStr"></param>
        /// <param name="whereParam"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public virtual async Task<DataTableWithTotalCount> FindTable(string sql, object sqlParams, PagedAndSortedInputDto pagination, string whereStr = null, object whereParam = null)
        {
            StringBuilder sqlbuf = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(whereStr))
            {
                if (sql.Contains("WHERE"))
                    sqlbuf.Append($" AND {whereStr}");
                else
                    sqlbuf.Append($" WHERE {whereStr}");
            }
            //是否有排序值，没有默认给一个排序，否则分页报错
            if (!string.IsNullOrWhiteSpace(pagination.Sorting))
            {
                sqlbuf.Append($" ORDER BY  {pagination.Sorting}");
            }
            else
            {
                sqlbuf.Append($" ORDER BY (select 0) ");
            }

            var skipCount = pagination.PageSize * (pagination.PageNumber - 1);
            sqlbuf.Append($" OFFSET {skipCount} ROWS");
            sqlbuf.Append($" FETCH NEXT {pagination.PageSize} ROWS ONLY");

            var dataTable = new DataTable();
            var totalCount = 0;
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlbuf.ToString();
                    if (sqlParams != null)
                    {
                        var dictionary = sqlParams as IDictionary;
                        if (dictionary != null)
                        {
                            foreach (DictionaryEntry param in dictionary)
                            {
                                var dbParameter = command.CreateParameter();
                                dbParameter.ParameterName = param.Key.ToString();
                                dbParameter.Value = param.Value;
                                command.Parameters.Add(dbParameter);
                            }
                        }
                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                            var countCommand = connection.CreateCommand();
                            countCommand.CommandText = $"SELECT COUNT(*) FROM ({sql}) AS TotalCount";
                            if (dictionary != null)
                            {
                                foreach (DictionaryEntry param in dictionary)
                                {
                                    var dbParameter = countCommand.CreateParameter();
                                    dbParameter.ParameterName = param.Key.ToString();
                                    dbParameter.Value = param.Value;
                                    countCommand.Parameters.Add(dbParameter);
                                }
                            }
                            totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                        }
                    }
                    // 显式关闭连接
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return new DataTableWithTotalCount { DataTable = dataTable, TotalCount = totalCount };
                }
            }
        }

        /// <summary>
        /// 获取所有表信息
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public virtual async Task<List<DbTableInfo>> GetTableInfoList()
        {
            List<DbTableInfo> dbTables = new List<DbTableInfo>();
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }
                using (var command = connection.CreateCommand())
                {
                    var sql = @" SELECT s.Name,Convert(nvarchar(max),tbp.value) as Description 
                                 FROM sysobjects s 
					     	     LEFT JOIN sys.extended_properties as tbp  
                                 ON s.id=tbp.major_id and tbp.minor_id=0 AND (tbp.Name='MS_Description' OR tbp.Name is null)   
                                 WHERE s.xtype IN('U') ";
                    command.CommandText = sql.ToString();
                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            string tableName = reader["Name"].ToString();
                            string tableDescription = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "";

                            dbTables.Add(new DbTableInfo
                            {
                                Name = tableName,
                                Description = tableDescription
                            });
                        }
                    }
                }

                // 显式关闭连接
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                #region
                //var tables = connection.GetSchema("Tables");

                //var v = tables.Rows.Cast<DataRow>();
                //foreach (var item in v)
                //{
                //    int i = 0;
                //}
                //var result = tables.Rows.Cast<DataRow>().Select(row => new DbTableInfo
                //{
                //    Name = row["TABLE_NAME"].ToString(),
                //    // Description = row["TABLE_COMMENT"] != DBNull.Value ? row["TABLE_COMMENT"].ToString() : ""

                //    Description = row["DESCRIPTION"] != DBNull.Value ? row["DESCRIPTION"].ToString() : ""

                //}).ToList();
                #endregion

                return dbTables;
            }
        }

        /// <summary>
        /// 获取指定表栏位详细信息
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public virtual async Task<List<DbColumnInfo>> GetColumnInfosByTableName(string TableName)
        {
            List<DbColumnInfo> dbTables = new List<DbColumnInfo>();
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }
                using (var command = connection.CreateCommand())
                {
                    var sql = string.Format(GetColumnInfosByTableNameSql, TableName);
                    command.CommandText = sql.ToString();
                    using (var reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            dbTables.Add(new DbColumnInfo
                            {
                                TableName = reader["TableName"].ToString(),
                                TableId = Convert.ToInt32(reader["TableId"]),
                                DbColumnName = reader["DbColumnName"].ToString(),
                                DataType = reader["DataType"].ToString(),
                                Length = Convert.ToInt32(reader["Length"]),
                                Scale = Convert.ToInt32(reader["Scale"]),
                                DecimalDigits = Convert.ToInt32(reader["DecimalDigits"]),
                                ColumnDescription = reader["ColumnDescription"].ToString(),
                                DefaultValue = reader["DefaultValue"].ToString(),
                                IsNullable = Convert.ToBoolean(reader["IsNullable"]),
                                IsIdentity = Convert.ToBoolean(reader["IsIdentity"]),
                                IsPrimarykey = Convert.ToBoolean(reader["IsPrimarykey"]),
                            });
                        }
                    }
                }
                return dbTables;
            }
        }

        /// <summary>
        /// 获取表栏位信息
        /// </summary>
        protected string GetColumnInfosByTableNameSql
        {
            get
            {
                string sql = @"SELECT sysobjects.name AS TableName,
                           syscolumns.Id AS TableId,
                           syscolumns.name AS DbColumnName,
                           systypes.name AS DataType,
                           COLUMNPROPERTY(syscolumns.id,syscolumns.name,'PRECISION') as [length],
                           isnull(COLUMNPROPERTY(syscolumns.id,syscolumns.name,'Scale'),0) as Scale, 
						   isnull(COLUMNPROPERTY(syscolumns.id,syscolumns.name,'Scale'),0) as DecimalDigits,
                           Cast( sys.extended_properties.[value] as nvarchar(2000)) AS [ColumnDescription],
                           syscomments.text AS DefaultValue,
                           syscolumns.isnullable AS IsNullable,
	                       columnproperty(syscolumns.id,syscolumns.name,'IsIdentity')as IsIdentity,
                           (CASE
                                WHEN EXISTS
                                       ( 
                                             	select 1
												from sysindexes i
												join sysindexkeys k on i.id = k.id and i.indid = k.indid
												join sysobjects o on i.id = o.id
												join syscolumns c on i.id=c.id and k.colid = c.colid
												where o.xtype = 'U' 
												and exists(select 1 from sysobjects where xtype = 'PK' and name = i.name) 
												and o.name=sysobjects.name and c.name=syscolumns.name
                                       ) THEN 1
                                ELSE 0
                            END) AS IsPrimaryKey
                    FROM syscolumns
                    INNER JOIN systypes ON syscolumns.xtype = systypes.xtype
                    LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id
                    LEFT OUTER JOIN sys.extended_properties ON (sys.extended_properties.minor_id = syscolumns.colid
                                                                AND sys.extended_properties.major_id = syscolumns.id)
                    LEFT OUTER JOIN syscomments ON syscolumns.cdefault = syscomments.id
                    WHERE syscolumns.id IN
                        (SELECT id
                         FROM sysobjects
                         WHERE upper(xtype) IN('U',
                                        'V') )
                      AND (systypes.name <> 'sysname')
                      AND sysobjects.name=N'{0}'
                      AND systypes.name<>'geometry'
                      AND systypes.name<>'geography'
                    ORDER BY syscolumns.colid";
                return sql;
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <returns></returns>
     
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public virtual async Task<object> ExecuteSql(string sql, object sqlParams)
        {
            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();
            using (var connection = context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = sql.ToString();
                        if (sqlParams != null)
                        {
                            var dictionary = sqlParams as IDictionary;
                            if (dictionary != null)
                            {
                                foreach (DictionaryEntry param in dictionary)
                                {
                                    var dbParameter = command.CreateParameter();
                                    dbParameter.ParameterName = param.Key.ToString();
                                    dbParameter.Value = param.Value ?? DBNull.Value;
                                    command.Parameters.Add(dbParameter);
                                }
                            }
                        }
                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }
                        var res = await command.ExecuteScalarAsync();
                        // 显式关闭连接
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        return res;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
        #region 判断数据表字段重复
        /// <summary>
        /// 格式sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string SetSql(string sql)
        {
            sql = Regex.Replace(sql, "[a-z,A-Z,0-9,.]*{ADTO_SASSID_NOTA}", " 1=1 ");
            sql = sql.Replace("{ADTO_SASSID}", "");
            sql = sql.Replace("{ADTO_SASSID_NOTT}", "");
            return sql;
        }
        /// <summary>
        /// 判断数据表字段重复-
        /// 对应data/dbtable/exist/filed
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="tableName">表名</param>
        /// <param name="keyName">主键名</param>
        /// <param name="filedsJson">数据字段</param>
        /// <param name="isNotSass">不需要多租户</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public async Task<bool> ExistFiled(GetExistFiledInput input)
        {
            string tableName = SqlFiltersCommon.SqlFilters(input.TableName);
            string keyName = SqlFiltersCommon.SqlFilters(input.KeyName);
            string filedsJson = SqlFiltersCommon.SqlFilters(input.FiledsJson);

            StringBuilder strSql = new StringBuilder();
            var tableNameList = tableName.Split(".");
            var dbCode = string.Empty;
            if (tableNameList.Length == 2)
            {
                dbCode = tableNameList[0];
                tableName = tableNameList[1];
            }
            strSql.Append("select * from " + tableName + " t where 1=1  ");
            if (!string.IsNullOrEmpty(input.KeyValue))
            {
                strSql.Append(" AND " + keyName + " !=@keyValue ");
            }
            var args = filedsJson.ToJObject();
            string argsstr = string.Empty;
            foreach (var item in args.Properties())
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    argsstr += " AND " + item.Name + " = '" + item.Value + "'";

                }
            }
            //若是查重的数据字段都为空，直接返回true
            if (string.IsNullOrEmpty(argsstr))
            {
                return true;
            }
            else
            {
                strSql.Append(argsstr);
            }
            var dataTable = new DataTable();

            var _dbContextProvider = IocManager.Instance.Resolve<IDbContextProvider<DCloudDbContext>>();
            var context = await _dbContextProvider.GetDbContextAsync();

            using (var connection = context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = strSql.ToString();
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = "keyValue";
                    dbParameter.Value = input.KeyValue;
                    command.Parameters.Add(dbParameter);

                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                    // 显式关闭连接
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

            if (dataTable.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
    public class DataTableWithTotalCount
    {
        public DataTable DataTable { get; set; }
        public int TotalCount { get; set; }
    }

}


