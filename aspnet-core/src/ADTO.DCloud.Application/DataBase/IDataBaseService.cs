using ADTO.DCloud.DataBase.Model;
using ADTO.DCloud.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ADTO.DCloud.DataBase
{
    public interface IDataBaseService
    {
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="whereText"></param>
        /// <returns></returns>
        [DisableAuditing, UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        Task<IEnumerable<string>> GetSqlColName(string sqlText, string whereText = null);

        /// <summary>
        /// 执行sql 语句,查询表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        Task<DataTable> FindTable(string sql, object sqlParams = null);

        /// <summary>
        /// 执行sql 语句,查询表数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="whereStr"></param>
        /// <param name="orderSql"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        Task<DataTable> FindTable(string sql, string whereStr, string orderSql, object sqlParams = null);

        /// <summary>
        /// 获取所有表信息
        /// </summary>
        /// <returns></returns>
        Task<List<DbTableInfo>> GetTableInfoList();

        /// <summary>
        /// 获取指定表栏位详细信息
        /// </summary>
        /// <returns></returns>
        Task<List<DbColumnInfo>> GetColumnInfosByTableName(string TableName);

        /// <summary>
        /// 执行sql 语句,查询表分页数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParams"></param>
        /// <param name="pagination"></param>
        /// <param name="whereStr"></param>
        /// <param name="whereParam"></param>
        /// <returns></returns>
        Task<DataTableWithTotalCount> FindTable(string sql, object sqlParams, PagedAndSortedInputDto pagination, string whereStr = null,  object whereParam = null);
    }
}
