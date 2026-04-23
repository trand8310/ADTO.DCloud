using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.DataSource.Dto;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using JavaScriptEngineSwitcher.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataSource
{
    /// <summary>
    /// 数据源\数据视图 相关方法
    /// </summary>
    public class DataSourceAppService : DCloudAppServiceBase, IDataSourceAppService
    {
        private readonly IRepository<DataSource, Guid> _dataSourceRepository;
        private readonly IDataBaseService _dataBaseService;
        public DataSourceAppService(
              IRepository<DataSource, Guid> dataSourceRepository
            , IDataBaseService dataBaseService)
        {
            _dataSourceRepository = dataSourceRepository;
            _dataBaseService = dataBaseService;
        }
        /// <summary>
        /// 添加数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateDataSourceAsync(CreateDataSourceInputDto input)
        {
            var existInfo = await this._dataSourceRepository.GetAll().Where(p => p.Code == input.Code).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,编码重复！");
            }
            var info = ObjectMapper.Map<DataSource>(input);
            if (!string.IsNullOrEmpty(input.Sql))
            {
                info.Sql = AESHelper.AesDecrypt(input.Sql, "ADTODCloud");//"12345678901234567890123456789012"
            }
            info.Sql = SqlFiltersCommon.SqlFiltersNotSelect(info.Sql);

            await _dataSourceRepository.InsertAsync(info);
        }

        /// <summary>
        /// 修改数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateDataSourceAsync(CreateDataSourceInputDto input)
        {
            var existInfo = await this._dataSourceRepository.GetAll().Where(p => p.Code == input.Code && p.Id != input.Id).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,编码重复！");
            }
            var info = this._dataSourceRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);
            if (!string.IsNullOrEmpty(input.Sql))
            {
                info.Sql = AESHelper.AesDecrypt(input.Sql, "ADTODCloud");//"12345678901234567890123456789012"
            }
            info.Sql = SqlFiltersCommon.SqlFiltersNotSelect(info.Sql);
            //转换一下，否则其它字段也会置空
            await _dataSourceRepository.UpdateAsync(info);
        }

        /// <summary>
        /// 获取指定数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataSourceDto> GetDataSourceByIdAsync(EntityDto<Guid> input)
        {
            var role = await _dataSourceRepository.GetAsync(input.Id);
            return ObjectMapper.Map<DataSourceDto>(role);
        }

        /// <summary>
        /// 删除指定的数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteDataSourceAsync(EntityDto<Guid> input)
        {
            await _dataSourceRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取数据源\数据视图 分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<DataSourceDto>> GetDataSourcePageList(PagedDataSourceResultRequestDto input)
        {
            var query = _dataSourceRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Name.Contains(input.KeyWord) || p.Code.Contains(input.KeyWord));

            var totalCount = await query.CountAsync();
            var infos = await query.PageBy(input).ToListAsync();
            var list = infos.Select(i =>
            {
                var dto = ObjectMapper.Map<DataSourceDto>(i);
                return dto;
            }).ToList();
            return new PagedResultDto<DataSourceDto>(totalCount, list);
        }

        /// <summary>
        /// 根据编码获取实体
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        public async Task<DataSourceDto> GetDataSourceByCodeAsync(DataSourceQueryDto input)
        {
            var role = await _dataSourceRepository.GetAll().Where(p => p.Code == input.Code).FirstOrDefaultAsync();
            return ObjectMapper.Map<DataSourceDto>(role);
        }

        /// <summary>
        /// 获取数据源数据(根据指定字段和值)
        /// </summary>
        /// <param name="code">数据源编号</param>
        /// <param name="field">字段名</param>
        /// <param name="dto">字段值</param>
        /// <returns></returns>
        //[HttpPost("data/dbsource/{code}/{field}/list")]
        [HttpPost]
        public async Task<DataTable> GetDataTableByValues(GetDataTableByValuesInput input)
        {
            var info = await this._dataSourceRepository.FirstOrDefaultAsync(p => p.Code == input.Code);
            if (info == null)
                return new DataTable();
            string whereSql = string.Empty;
            var userInfo = await UserManager.FindByIdAsync(ADTOSharpSession.GetUserId().ToString());
            string sql = $" {input.Field} in ('{input.Ids.Replace(",", "','")}') ";

            //if (info.DbId == "learun_view")
            //{
            //    DataSourceModel model = info.Sql.ToObject<DataSourceModel>();
            //    DataSourceRes dbSource = await GetViewRes(model, input.paramsJson, userInfo);
            //    return await GetDataViews(dbSource, sql);
            //}
            //else
            //{

            DataSourceModel model = info.Sql.ToObject<DataSourceModel>();
            DataSourceRes dbSource = await GetViewRes(model, input.paramsJson, userInfo);
            dbSource.Order = !string.IsNullOrEmpty(dbSource.Order) && !dbSource.Order.Contains("order by") ? $" order by {dbSource.Order}" : dbSource.Order;
            return await GetDataViews(dbSource, sql);

            //return await GetDataTable(info, input.paramsJson, sql, userInfo);
            //}
        }
        /// <summary>
        /// 获取数据源列名
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDataColName(string sql)
        {
            sql = System.Text.RegularExpressions.Regex.Replace(sql, "@[a-z,A-Z,0-9]+", "''");
            return await _dataBaseService.GetSqlColName(sql);
        }

        /// <summary>
        /// 获取数据源列名 data/dbsource/{code}/colnames" 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDataColNameByCode(string code)
        {
            var info = await this._dataSourceRepository.GetAll().Where(p => p.Code == code).FirstOrDefaultAsync();
            if (info == null)
            {
                return new List<string>();
            }
            DataSourceModel model = info.Sql.ToObject<DataSourceModel>();
            List<string> res = new List<string>();
            foreach (var col in model.Columns)
            {
                res.Add(col.Prop);
            }
            return res;
        }

        /// <summary>
        /// 获取数据源的数据(分页) data/dbsource/view/page  post方式
        /// data/dbsource/view/page
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageReturnDataTableDto<Dictionary<string, object>>> GetDataTableViewPage(GetDataTablePageQueryDto input)
        {
            var data = await GetDataTable(input);
            var cols = GetDataColumnsInfo(data);

            var list = data.AsEnumerable()
                .Select(r => data.Columns.Cast<DataColumn>()
                    .ToDictionary(col => char.ToLower(col.ColumnName[0], CultureInfo.InvariantCulture) + col.ColumnName.Substring(1), col => r[col])
            ).ToList();
            return new PageReturnDataTableDto<Dictionary<string, object>>(input.records, list, cols);
        }



        #region 获取数据源数据 data/dbsource/{code}/page
        /// <summary>
        /// 获取数据源的数据(分页) data/dbsource/{code}/page
        /// data/dbsource/view/page
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageReturnDataTableDto<Dictionary<string, object>>> GetDataTablePage(GetDataTablePageQueryDto input)
        {
            var data = await GetDataTable(input);
            var cols = GetDataColumnsInfo(data);

            var list = data.AsEnumerable()
                .Select(r => data.Columns.Cast<DataColumn>()
                    .ToDictionary(col => char.ToLower(col.ColumnName[0], CultureInfo.InvariantCulture) + col.ColumnName.Substring(1), col => r[col])
            ).ToList();
            return new PageReturnDataTableDto<Dictionary<string, object>>(input.records, list, cols);
        }

        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="dt">数据集合</param>
        /// <returns></returns>
        public List<DataSourceColumns> GetDataColumnsInfo(DataTable dt)
        {
            var res = new List<DataSourceColumns>();
            foreach (DataColumn col in dt.Columns)
            {
                res.Add(new DataSourceColumns()
                {
                    Prop = char.ToLower(col.ColumnName[0], CultureInfo.InvariantCulture) + col.ColumnName.Substring(1),
                    Label = col.ColumnName,
                    CsType = col.DataType.Name
                });
            }
            return res;
        }


        /// <summary>
        /// 获取数据源的数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<DataTable> GetDataTable(GetDataTablePageQueryDto input)
        {
            var userInfo = await UserManager.FindByIdAsync(ADTOSharpSession.GetUserId().ToString());
            input.whereSql = SqlFiltersCommon.SqlFilters(input.whereSql);

            DataSource entity = new DataSource();
            // 数据视图配置了，还未保存，不存在编码 data/dbsource/view/page
            if (!string.IsNullOrWhiteSpace(input.querySql))
            {
                entity.Sql = AESHelper.AesDecrypt(input.querySql, "ADTODCloud");
                //entity.Sql = input.querySql;

            }
            else
            {
                entity = await this._dataSourceRepository.GetAll().Where(p => p.Code == input.Code).FirstOrDefaultAsync();
            }

            //if (userInfo.SecurityLevel != 1)
            //{
            //    var authWhereSql = await this.GetDataAuthoritySql(entity.F_Id);
            //    if (string.IsNullOrEmpty(whereSql))
            //    {
            //        whereSql = authWhereSql;
            //    }
            //    else if (!string.IsNullOrEmpty(authWhereSql))
            //    {
            //        whereSql = $"({whereSql}) AND ({authWhereSql})";
            //    }
            //}
            //if (entity.DbId == "learun_view")
            //{
            DataSourceModel model = entity.Sql.ToObject<DataSourceModel>();
            DataSourceRes dbSource = await GetViewRes(model, input.param, userInfo);
            return await GetDataViewPage(dbSource, input);
            //}
            //else
            //{
            //    return await dataSourceService.GetDataTable(entity, pagination, paramsJson, whereSql, userInfo);
            //}
        }

        #endregion
        private async Task<DataTable> GetDataViewPage(DataSourceRes dbSource, GetDataTablePageQueryDto input)
        {
            if (!string.IsNullOrEmpty(dbSource.Sql))
            {
                //if (string.IsNullOrEmpty(input..sidx) && !string.IsNullOrEmpty(dbSource.Order))
                //{
                //    pagination.sidx = dbSource.Order;
                //}

                if (dbSource.Sql.ToUpper().Contains("WITH"))//WITH查询-不分页
                {
                    return await _dataBaseService.FindTable(dbSource.Sql, input.whereSql, dbSource.Order, dbSource.ParamsDic);
                }
                else
                {
                    PagedAndSortedInputDto requestDto = new PagedAndSortedInputDto() { PageNumber = input.PageNumber, PageSize = input.PageSize, Sorting = input.Sorting };
                    var vList = await _dataBaseService.FindTable(dbSource.Sql, dbSource.ParamsDic, requestDto, input.whereSql);
                    input.records = vList.TotalCount;
                    return vList.DataTable;

                }
            }
            else
            {
                DataTable res = dbSource.Data;
                if (!string.IsNullOrEmpty(input.whereSql))
                {
                    DataRow[] drs = res.Select(input.whereSql);
                    DataTable nDt = res.Clone();
                    foreach (DataRow dr in drs)
                    {
                        nDt.ImportRow(dr);
                    }
                    return nDt;
                }
                if (!string.IsNullOrEmpty(dbSource.Order))
                {
                    var dv = res.DefaultView;
                    dv.Sort = dbSource.Order;
                    res = dv.ToTable();
                }
                //else if (!string.IsNullOrEmpty(pagination.sidx))
                //{
                //    var dv = res.DefaultView;
                //    dv.Sort = pagination.sidx;
                //    res = dv.ToTable();
                //}
                input.records = res.Rows.Count;

                if (res.Rows.Count <= input.PageSize)
                {
                    return res;
                }
                else
                {
                    DataTable newDt = res.Clone();
                    int bnum = (input.PageNumber - 1) * input.PageSize;
                    int endnum = bnum + input.PageSize;
                    if (endnum > res.Rows.Count)
                    {
                        endnum = res.Rows.Count;
                    }
                    for (var i = bnum; i < endnum; i++)
                    {
                        newDt.ImportRow(res.Rows[i]);
                    }
                    return newDt;
                }
            }
        }

        /// <summary>
        /// 获取数据源的数据(表单设计数据视图绑定控件数据时，获取所有的数据)
        /// data/dbsource/{code}/list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataTable> GettDataSourceTable(GetDataTableQueryDto input)
        {
            try
            {
                var userInfo = await UserManager.FindByIdAsync(ADTOSharpSession.GetUserId().ToString());
                string whereSql = string.Empty;

                var entity = await this._dataSourceRepository.GetAll().Where(p => p.Code == input.Code).FirstOrDefaultAsync();
                //if (userInfo.SecurityLevel != 1)
                //{
                //    whereSql = await this.GetDataAuthoritySql(entity.F_Id);
                //}
                //if (entity.DbId == "learun_view") 查看新增数据源，三种类型都是传的同一个值，所以暂时取消判断
                //{
                DataSourceModel model = entity.Sql.ToObject<DataSourceModel>();
                DataSourceRes dbSource = await GetViewRes(model, input.paramsJson, userInfo);

                if (!string.IsNullOrEmpty(input.sidx))
                {
                    dbSource.Order = input.sidx;
                }
                dbSource.Order = !string.IsNullOrEmpty(dbSource.Order) && !dbSource.Order.Contains("order by") ? $" order by {dbSource.Order}" : dbSource.Order;
                return await GetDataViews(dbSource, whereSql);
                //}
                //else
                //{
                //    string param = "";
                //    if (!string.IsNullOrEmpty(input.paramsJson))
                //    {
                //        var paramsDic = input.paramsJson.ToObject<Dictionary<string, object>>();
                //        if (paramsDic.ContainsKey("param"))
                //        {
                //            param = paramsDic["param"].ToString();
                //        }
                //    }
                //    return await GetDataTableByParamter(entity, param, whereSql, userInfo);
                //}
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }







        #region 获取数据源数据相关辅助方法

        /// <summary>
        /// 获取数据源的数据 (有參數)
        /// </summary>
        /// <param name="entity">数据源实体</param>
        /// <param name="param">参数</param>
        /// <param name="whereSql">条件语句</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        private async Task<DataTable> GetDataTableByParamter(DataSource entity, string param, string whereSql, User userInfo)
        {
            if (entity == null)
            {
                return new DataTable();
            }
            else
            {
                if (entity.DbId == "learun_new_data")
                {
                    DataSourceModel model = entity.Sql.ToObject<DataSourceModel>();
                    return await GetDataList(model, new { param }.ToJson(), whereSql, userInfo);
                }
                else
                {
                    string sql = entity.Sql;
                    //sql = sql.Replace("{userId}", $"'{userInfo.UserId}'");
                    //sql = sql.Replace("{userAccount}", $"'{userInfo.F_Account}'");
                    //sql = sql.Replace("{companyId}", $"'{userInfo.F_CompanyId}'");
                    //sql = sql.Replace("{departmentId}", $"'{userInfo.F_DepartmentId}'");
                    return await _dataBaseService.FindTable(sql, whereSql, "", new { param });

                }
            }
        }

        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="model">配置模型信息</param>
        /// <param name="paramsObject">参数对象</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        private async Task<DataTable> GetDataList(DataSourceModel model, string paramsObject, string whereSql, User userInfo)
        {
            var paramsDic = GetParams(model, paramsObject, userInfo, "");
            var order = GetSqlByOrder(model);

            if (model.Sqls.Count == 1)
            {
                if (!string.IsNullOrEmpty(order))
                {
                    model.Sqls[0].Sql = $"{model.Sqls[0].Sql} order by {order}";
                }
                return await _dataBaseService.FindTable(model.Sqls[0].Sql, whereSql, "", paramsDic);
            }
            else
            {
                var res = await GetMoreDbData(model, paramsDic, whereSql);
                if (!string.IsNullOrEmpty(order))
                {
                    var dv = res.DefaultView;
                    dv.Sort = order;
                    return dv.ToTable();
                }
                return res;
            }
        }
        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="dbSource">数据源对象</param>
        /// <param name="whereSql">查询语句</param>
        /// <returns></returns>
        private async Task<DataTable> GetDataViews(DataSourceRes dbSource, string whereSql)
        {
            if (!string.IsNullOrEmpty(dbSource.Sql))
            {
                //重新构建sql语句-当参数为空时查询全部信息
                if (!dbSource.ParamsDic.ContainsKey("param"))//存在参数
                {
                    dbSource.Sql = dbSource.Sql.Replace("= @param", " is not null");
                    dbSource.Sql = dbSource.Sql.Replace("=@param", " is not null");
                }

                return await _dataBaseService.FindTable(dbSource.Sql, whereSql, dbSource.Order, dbSource.ParamsDic);
            }
            else
            {
                DataTable res = dbSource.Data;

                if (!string.IsNullOrEmpty(whereSql))
                {
                    DataRow[] drs = res.Select(whereSql);
                    DataTable nDt = res.Clone();
                    foreach (DataRow dr in drs)
                    {
                        nDt.ImportRow(dr);
                    }
                    return nDt;
                }

                if (!string.IsNullOrEmpty(dbSource.Order))
                {
                    var dv = res.DefaultView;
                    dv.Sort = dbSource.Order;
                    res = dv.ToTable();
                }

                return res;
            }
        }

        /// <summary>
        /// 获取数据视图
        /// </summary>
        /// <param name="model">配置信息</param>
        /// <param name="paramsObject">参数信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        private async Task<DataSourceRes> GetViewRes(DataSourceModel model, string paramsObject, User userInfo, string param = "")
        {
            var res = new DataSourceRes();
            res.ParamsDic = GetParams(model, paramsObject, userInfo, param);
            res.Order = GetSqlByOrder(model);

            var configSql = model.Sqls[0];
            res.DbCode = configSql.DbCode;
            switch (model.Type)
            {
                case "table": // 执行表数据
                    //res.Sql = $"select * from {configSql.DbTable} where " + " {LEARUN_SASSID_NOTA} ";
                    res.Sql = $"select * from {configSql.DbTable}  ";
                    break;
                case "code":
                    // 测试后端执行js方法
                    res.Data = await JsDataView(model, res.ParamsDic);
                    break;
                default:
                    if (model.Sqls.Count > 1)
                    {
                        //暂时去掉
                        //await AddChildrenSqlParams(model, res.ParamsDic);
                    }
                    res.Sql = configSql.Sql;

                    //foreach (var item in model.QueryFields)
                    //{
                    //    if (res.ParamsDic.TryGetValue(item.Name, out object value) && value != null)
                    //    {
                    //        if (res.ParamsDic.ContainsKey(item.Name))
                    //            res.Sql += $" and {item.Name} = @{item.Name}";
                    //        else if (!res.Sql.Contains("where"))
                    //            res.Sql += $" where {item.Name} = @{item.Name}";
                    //    }

                    //}

                    //重新构建sql语句-当参数为空时查询全部信息
                    if (!res.ParamsDic.ContainsKey("param"))//存在参数
                    {
                        res.Sql = res.Sql.Replace("= @param", " is not null");
                        res.Sql = res.Sql.Replace("=@param", " is not null");

                        res.Sql = res.Sql.Replace("@param", "''");
                    }
                    break;
            }
            return res;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="model">设置模型</param>
        /// <param name="paramsObject">参数对象</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        private Dictionary<string, object> GetParams(DataSourceModel model, string paramsObject, User userInfo, string param)
        {
            Dictionary<string, object> paramsDic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(paramsObject))
            {
                paramsDic = paramsObject.ToObject<Dictionary<string, object>>();
            }
            // 获取参数
            if (model.QueryFields != null)
            {
                foreach (var item in model.QueryFields)
                {
                    if (!paramsDic.ContainsKey(item.Name))
                    {
                        paramsDic.Add(item.Name, GetDbParameter(item.Default, item.Type));
                    }
                    else
                    {
                        paramsDic[item.Name] = GetDbParameter(paramsDic[item.Name].ToString(), item.Type);
                    }
                }
            }

            if (!paramsDic.ContainsKey("param") && !string.IsNullOrEmpty(param))
            {
                paramsDic.Add("param", param);
            }

            if (!paramsDic.ContainsKey("userId"))
            {
                paramsDic.Add("userId", userInfo.Id);
            }

            if (!paramsDic.ContainsKey("userAccount"))
            {
                paramsDic.Add("userAccount", userInfo.UserName);
            }

            if (!paramsDic.ContainsKey("companyId"))
            {
                // paramsDic.Add("companyId", userInfo.F_CompanyId);
            }

            if (!paramsDic.ContainsKey("departmentId"))
            {
                // paramsDic.Add("departmentId", userInfo.F_DepartmentId);
            }
            ///默认加上keyword参数
            if (!paramsDic.ContainsKey("keyword"))
            {
                paramsDic.Add("keyword", "");
            }

            return paramsDic;
        }
        // 获取值，如果键不存在或值为 null，则返回默认值
        object GetValueOrDefault(Dictionary<string, object> paramsDic,string key)
        {
            return paramsDic.TryGetValue(key, out object val) ? val : null;
        }
        /// <summary>
        /// 给sql添加排序语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetSqlByOrder(DataSourceModel model)
        {
            string res = "";
            if (model.Orders != null)
            {
                foreach (var item in model.Orders)
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        res += ",";
                    }

                    res += item.Field;
                    if (item.IsDESC)
                    {
                        res += " DESC";
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 执行js脚本
        /// </summary>
        /// <param name="model">设置数据</param>
        /// <param name="paramsDic">参数信息</param>
        /// <returns></returns>
        private async Task<DataTable> JsDataView(DataSourceModel model, Dictionary<string, object> paramsDic)
        {
            string code = model.Sqls[0].Sql;
            Dictionary<string, DataTable> paramsData = new Dictionary<string, DataTable>();
            foreach (var dataSourceInput in model.DataSourceInputs)
            {
                // 参数传递
                Dictionary<string, object> selectParamsDic = new Dictionary<string, object>();
                foreach (var key in paramsDic.Keys)
                {
                    if (key.StartsWith($"{dataSourceInput.Code}_"))
                    {
                        selectParamsDic.Add(key, paramsDic[key]);
                    }
                }
                GetDataTableQueryDto queryDto = new GetDataTableQueryDto()
                {
                    Code = dataSourceInput.Code,
                    paramsJson = selectParamsDic.ToJson(),
                    sidx = ""
                };
                var data = await GettDataSourceTable(queryDto);
                //var data = await GetDataTable(dataSourceInput.Code, "", selectParamsDic.ToJson());
                paramsData.Add(dataSourceInput.Code, data);
            }
            paramsDic.Add("data", paramsData);
            var jsonData = paramsDic.ToJson();
            IJsEngine engine = JsEngineSwitcher.Current.CreateDefaultEngine(); //new JurassicJsEngine();
            engine.SetVariableValue("inputStrParams", jsonData);
            string jsCode = "function fn(strLearun) { var learun = JSON.parse(strLearun);  " + code + " };var res = JSON.stringify(fn(inputStrParams) || []);";
            engine.Execute(jsCode);
            var result = engine.Evaluate<string>("res");

            // var learun = JSON.parse(strLearun); 
            // " + code + "

            return result.ToObject<DataTable>();
        }

        /// <summary>
        /// 获取子查询条件参数（跨库才需要）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="paramsDic"></param>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        private async Task AddChildrenSqlParams(DataSourceModel model, Dictionary<string, object> paramsDic)
        {
            // 先进行子查询查询，用来作为参数给父级查询
            var childSqls = model.Sqls.FindAll(t => t.Type == "2");
            foreach (var itemSql in childSqls)
            {
                var dt = await _dataBaseService.FindTable(itemSql.Sql, paramsDic);

                List<string> childParams = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    childParams.Add(dr[0].ToString());
                }

                if (!paramsDic.ContainsKey(itemSql.ParamName))
                {
                    paramsDic.Add(itemSql.ParamName, childParams.ToArray());
                }
            }
        }

        private object GetDbParameter(string value, string csType)
        {
            // 特殊处理 like 类型（直接返回带百分号的字符串，不进行类型转换）
            if (csType == "like")
            {
                return string.IsNullOrEmpty(value) ? "%%" : $"%{value}%";
            }
            string typeName = "";
            object defaultValue = DBNull.Value;   // 存储实际类型的默认值（非字符串形式）
            switch (csType)
            {
                case "int":
                    typeName = "System.Int32";
                    defaultValue = 0;
                    break;
                case "DateTime":
                    typeName = "System.DateTime";
                    defaultValue = new DateTime(1000, 1, 1, 1, 0, 0);// "1000-01-01 01:00:00";
                    break;
                case "Guid":
                    typeName = "System.Guid";
                    defaultValue = DBNull.Value;
                    break;
                default:
                    // 未识别的类型，直接返回原始字符串（或空字符串）
                    return value ?? "";
                    #region like
                    //case "like":
                    //    _csType = "";
                    //    if (string.IsNullOrEmpty(value))
                    //    {
                    //        value = "%%";
                    //    }
                    //    else
                    //    {
                    //        value = $"%{value}%";
                    //    }
                    //    break;
                    #endregion
            }

            // 如果传入值为空，则使用默认值
            if (string.IsNullOrEmpty(value))
            {
                // 默认值为 null 时，直接返回 null（上层需转为 DBNull.Value）
                if (defaultValue == null)
                    return null;
                // 否则将默认值转为字符串进行后续转换
                value = defaultValue.ToString();
            }

            // 获取目标类型
            Type targetType = Type.GetType(typeName);
            if (targetType == null)
                return value;   // 类型加载失败，回退返回字符串

            try
            {
                // 使用 TypeConverter 进行转换（支持 Guid 等复杂类型）
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFrom(value);
                }
                else
                {
                    return Convert.ChangeType(value, targetType);
                }
            }
            catch
            {
                // 转换失败时返回默认值（如 int 返回 0，DateTime 返回固定值，Guid 返回 null）
                return defaultValue;
            }
            //if (string.IsNullOrEmpty(value))
            //{
            //    value = defualtValue.ToString();
            //} 
            //return string.IsNullOrEmpty(_csType) ? value : Convert.ChangeType(value, System.Type.GetType(_csType));
        }

        /// <summary>
        /// 获取跨库查询数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="paramsDic"></param>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        private async Task<DataTable> GetMoreDbData(DataSourceModel model, Dictionary<string, object> paramsDic, string whereSql)
        {
            DataTable res = new DataTable();
            DataTable mainData = new DataTable();

            Dictionary<string, Dictionary<string, List<DataRow>>> drList = new Dictionary<string, Dictionary<string, List<DataRow>>>();
            Dictionary<string, DataTable> dtDic = new Dictionary<string, DataTable>();

            // 先进行子查询查询，用来作为参数给父级查询
            var childSqls = model.Sqls.FindAll(t => t.Type == "2");
            foreach (var itemSql in childSqls)
            {
                var dt = await _dataBaseService.FindTable(itemSql.Sql, paramsDic);

                List<string> childParams = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    childParams.Add(dr[0].ToString());
                }

                if (!paramsDic.ContainsKey(itemSql.ParamName))
                {
                    paramsDic.Add(itemSql.ParamName, childParams.ToArray());
                }
            }

            foreach (var item in model.Sqls)
            {
                if (item.Id == "1")
                {
                    mainData = await this._dataBaseService.FindTable(item.Sql, paramsDic);
                    SetCols(res, mainData);
                }
                else if (item.Type == "1")
                {
                    var dt = await _dataBaseService.FindTable(item.Sql, paramsDic);
                    SetCols(res, dt);
                    dtDic.Add(item.Id, dt);

                    var dicData = new Dictionary<string, List<DataRow>>();
                    drList.Add(item.Id, dicData);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var key = GetMyKey(dr, item.Relations);
                        if (!dicData.ContainsKey(key))
                        {
                            dicData.Add(key, new List<DataRow>());
                        }
                        dicData[key].Add(dr);
                    }
                }
            }

            foreach (DataRow mainDr in mainData.Rows)
            {
                List<DataRow> rowList = new List<DataRow>();
                var newDr = res.NewRow();
                rowList.Add(newDr);
                bool isAdd = true;
                foreach (var item in model.Sqls)
                {
                    if (item.Type != "2")
                    {
                        if (item.Id != "1")
                        {
                            List<DataRow> newRowList = new List<DataRow>();
                            foreach (var nDr in rowList)
                            {
                                var key = GetOtherKey(newDr, item.Relations);
                                if (!drList[item.Id].ContainsKey(key))
                                {
                                    isAdd = false;
                                    break;
                                }
                                var drs = drList[item.Id][key];
                                if (drs.Count == 0)
                                {
                                    isAdd = false;
                                    break;
                                }
                                else if (drs.Count == 1)
                                {
                                    SetRow(nDr, drs[0], dtDic[item.Id].Columns);
                                    newRowList.Add(nDr);
                                }
                                else
                                {
                                    foreach (var dr in drs)
                                    {
                                        var newDrCopy = res.NewRow();
                                        newDrCopy.ItemArray = (object[])nDr.ItemArray.Clone();
                                        SetRow(newDrCopy, dr, dtDic[item.Id].Columns);
                                        newRowList.Add(newDrCopy);
                                    }
                                }
                            }
                            if (!isAdd)
                            {
                                break;
                            }
                            rowList = newRowList;
                        }
                        else
                        {
                            SetRow(newDr, mainDr, mainData.Columns);
                        }
                    }
                }

                if (isAdd)
                {
                    foreach (var row in rowList)
                    {
                        res.Rows.Add(row);
                    }
                }
            }

            if (!string.IsNullOrEmpty(whereSql))
            {
                //whereSql = whereSql.Replace("like ","con")

                DataRow[] drs = res.Select(whereSql);
                DataTable nDt = res.Clone();
                foreach (DataRow dr in drs)
                {
                    nDt.ImportRow(dr);
                }
                return nDt;
            }

            return res;
        }
        /// <summary>
        /// 设置行值
        /// </summary>
        /// <param name="newDr"></param>
        /// <param name="dr"></param>
        /// <param name="cols"></param>
        private void SetRow(DataRow newDr, DataRow dr, DataColumnCollection cols)
        {
            for (int i = 0; i < cols.Count; i++)
            {
                newDr[cols[i].ColumnName] = dr[cols[i].ColumnName];
            }
        }

        /// <summary>
        /// 获取关联数据值
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        private string GetMyKey(DataRow dr, List<DataSourceRelationModel> relations)
        {
            string key = "";
            foreach (var item in relations)
            {
                key += $"{dr[item.MyField]}_";
            }
            return key;
        }

        /// <summary>
        /// datatable中添加列
        /// </summary>
        /// <param name="newDt"></param>
        /// <param name="dt"></param>
        private void SetCols(DataTable newDt, DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                newDt.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);// 需要加上数据类型，解决排序问题
            }
        }

        /// <summary>
        /// 获取关联数据值
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        private string GetOtherKey(DataRow dr, List<DataSourceRelationModel> relations)
        {
            string key = "";
            foreach (var item in relations)
            {
                key += $"{dr[item.OtherField]}_";
            }
            return key;
        }

        #endregion 

        #region 扩展方法
        /// <summary>
        /// 获取数据源的数据
        /// </summary>
        /// <param name="entity">数据源实体</param>
        /// <param name="paramsObject">查询参数</param>
        /// <param name="whereSql">数据权限sql</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        public async Task<DataTable> GetDataTable(DataSource entity, string paramsObject, string whereSql, User userInfo)
        {
            if (entity == null)
            {
                return new DataTable();
            }
            else
            {
                if (entity.DbId == "learun_new_data")
                {
                    DataSourceModel model = entity.Sql.ToObject<DataSourceModel>();
                    return await GetDataList(model, paramsObject, whereSql, userInfo);
                }
                else
                {
                    string sql = entity.Sql;
                    if (!string.IsNullOrEmpty(entity.Sql))
                    {
                        // 
                        sql = sql.Replace("{userId}", $"'{userInfo.Id}'");
                        sql = sql.Replace("{UserName}", $"'{userInfo.UserName}'");
                        sql = sql.Replace("{companyId}", $"'{userInfo.CompanyId}'");
                        sql = sql.Replace("{departmentId}", $"'{userInfo.DepartmentId}'");

                        sql = sql.Replace("=@param", " is not null");
                        sql = sql.Replace("= @param", " is not null");
                    }
                    var aa = GetTranslationColumnNameBySql(sql, 0, true);
                    return new DataTable();
                    //return await FindTable(sql, whereSql);
                }

            }
        }


        public static string GetTranslationColumnNameBySql(string sql, DbType dbType, bool pgSqlIsAutoToLower)
        {
            //IL_0044: Unknown result type (might be due to invalid IL or missing references)
            //IL_0045: Unknown result type (might be due to invalid IL or missing references)
            //IL_0047: Unknown result type (might be due to invalid IL or missing references)
            //IL_0049: Unknown result type (might be due to invalid IL or missing references)
            //IL_004b: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a6: Expected I4, but got Unknown
            if (sql.IndexOf("[") == -1 || sql.IndexOf("]") == -1)
            {
                return sql;
            }
            Regex regex = new Regex("\\[(\\w+)\\]");
            string text = string.Empty;
            string newValue = string.Empty;
            switch ((int)dbType)
            {
                case 0:
                case 2:
                case 19:
                case 20:
                    text = "`";
                    newValue = "`";
                    break;
                case 1:
                    text = "[";
                    newValue = "]";
                    break;
                case 3:
                case 5:
                case 6:
                    sql = regex.Replace(sql, (Match match) => match.Value.ToUpper());
                    text = "\"";
                    newValue = "\"";
                    break;
                case 4:
                case 10:
                    if (pgSqlIsAutoToLower)
                    {
                        sql = regex.Replace(sql, (Match match) => match.Value.ToLower());
                    }
                    sql = sql.Replace("{learun_sassid}", "{LEARUN_SASSID}");
                    sql = sql.Replace("{learun_sassid_nott}", "{LEARUN_SASSID_NOTT}");
                    sql = sql.Replace("{learun_sassid_nota}", "{LEARUN_SASSID_NOTA}");
                    text = "\"";
                    newValue = "\"";
                    break;
                case 7:
                case 11:
                case 12:
                case 13:
                    text = "\"";
                    newValue = "\"";
                    break;
            }
            if (text == "[")
            {
                return sql;
            }
            sql = sql.Replace("[", text).Replace("]", newValue);
            return sql;
        }

        #endregion
    }
}

