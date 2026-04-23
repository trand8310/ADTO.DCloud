using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.CodeTable;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.DataItem.Dto;
using ADTO.DCloud.FormScheme.Dto;
using ADTO.DCloud.FormScheme.Model; 
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTO.DCloud.WorkFlow.Processs.Config;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using AdvancedStringBuilder;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Installer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static ADTOSharp.Domain.Uow.ADTOSharpDataFilters;

namespace ADTO.DCloud.FormScheme
{
    /// <summary>
    /// 表单模板相关方法
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_FormDesign)]
    public class FormSchemeAppService : DCloudAppServiceBase, IFormSchemeAppService
    {
        private readonly IRepository<FormSchemeInfo, Guid> _formSchemeInfoRepository;
        private readonly IRepository<FormScheme, Guid> _formSchemeRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly DataBaseService _dataBaseService;
        private readonly InvokeMethodHelper _invokeMethodHelper;
        public FormSchemeAppService(IRepository<FormSchemeInfo, Guid> formSchemeInfoRepository, IRepository<FormScheme, Guid> formSchemeRepository, IRepository<User, Guid> userRepository,
            DataBaseService dataBaseService, InvokeMethodHelper invokeMethodHelper)
        {
            _formSchemeInfoRepository = formSchemeInfoRepository;
            _formSchemeRepository = formSchemeRepository;
            _userRepository = userRepository;
            _dataBaseService = dataBaseService;
            _invokeMethodHelper = invokeMethodHelper;
        }

        /// <summary>
        /// 新增表单设计(自动建表类型去除)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FormSchemeInfoDto> CreateFormScheme(CreateFormSchemeInputDto input)
        {
            //新增表单模板主表
            var vSchemeInfo = ObjectMapper.Map<FormSchemeInfo>(input.Info);
            var SchemeInfoId = await this._formSchemeInfoRepository.InsertAndGetIdAsync(vSchemeInfo);
            if (SchemeInfoId != Guid.Empty)
            {
                vSchemeInfo.Id = SchemeInfoId;
                //新增模板表
                input.Scheme.SchemeInfoId = SchemeInfoId;
                var vScheme = ObjectMapper.Map<FormScheme>(input.Scheme);
                var vSchemeId = await this._formSchemeRepository.InsertAndGetIdAsync(vScheme);

                if (vSchemeId != Guid.Empty)
                {
                    CurrentUnitOfWork.SaveChanges();
                    //修改关联模板主键(修改指定字段)
                    await this._formSchemeInfoRepository.UpdateAsync(SchemeInfoId, async entity => { entity.SchemeId = vSchemeId; });
                }
                else
                {
                    throw new UserFriendlyException("表单模板新增异常！");
                }
            }
            else
            {
                throw new UserFriendlyException("表单新增失败！");
            }

            return ObjectMapper.Map<FormSchemeInfoDto>(vSchemeInfo);
        }

        /// <summary>
        /// 修改表单设计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateFormScheme(CreateFormSchemeInputDto input)
        {
            if (input.Info != null && input.Info.Id != Guid.Empty)
            {
                //表单主表
                var vSchemeInfo = this._formSchemeInfoRepository.Get(input.Info.Id.Value);
                if (vSchemeInfo == null)
                {
                    throw new UserFriendlyException("修改失败：表单记录不存在！");
                }
                //查看模板表
                var schemeOldEntity = await this._formSchemeRepository.GetAsync(vSchemeInfo.SchemeId);
                //新增模板表(如果做了更新就新增一条模板表，没更新则不处理)
                if (schemeOldEntity == null || schemeOldEntity.Type != input.Scheme.Type || schemeOldEntity.Scheme != input.Scheme.Scheme)
                {
                    //新增模板表
                    input.Scheme.SchemeInfoId = input.Info.Id.Value;
                    var vScheme = ObjectMapper.Map<FormScheme>(input.Scheme);
                    var vSchemeId = await this._formSchemeRepository.InsertAndGetIdAsync(vScheme);

                    input.Info.SchemeId = vSchemeId;
                }

                //修改主表
                ObjectMapper.Map(input.Info, vSchemeInfo);
                await _formSchemeInfoRepository.UpdateAsync(vSchemeInfo);
            }
        }

        /// <summary>
        /// 获取表单主体信息custmerform/scheme/info/{id}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FormSchemeInfoDto> GetFormSchemeInfoById(EntityDto<Guid> input)
        {
            var formSchemeInfo = await this._formSchemeInfoRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            return ObjectMapper.Map<FormSchemeInfoDto>(formSchemeInfo);
        }

        /// <summary>
        /// 获取动态表单详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FormSchemeOutputDto> GetFormSchemInfo(EntityDto<Guid> input)
        {
            FormSchemeOutputDto outModelDto = new FormSchemeOutputDto();
            var infoModel = await this._formSchemeInfoRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (infoModel != null)
            {
                //表单主表
                outModelDto.Info = ObjectMapper.Map<FormSchemeInfoDto>(infoModel);
                //模板表
                var scheme = await this._formSchemeRepository.GetAsync(infoModel.SchemeId);
                outModelDto.Scheme = ObjectMapper.Map<FormSchemeDto>(scheme);
            }

            return outModelDto;
        }

        /// <summary>
        /// 获取动态表单模板分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<FormSchemeListShowDto>> GetFormSchemePageList(PagedFormSchemeResultRequestDto input)
        {
            var schemeInfo = _formSchemeInfoRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Name.Contains(input.KeyWord))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Category), p => p.Category == input.Category)
                .WhereIf(input.IsActive != null && input.IsActive == true, p => p.IsActive == true);

            //左链接
            var queryList = from p in schemeInfo
                            join f in _formSchemeRepository.GetAll() on p.SchemeId equals f.Id into p_f
                            from ff in p_f.DefaultIfEmpty()
                            join u in _userRepository.GetAll() on ff.CreatorUserId equals u.Id into f_u
                            from uu in f_u.DefaultIfEmpty()
                            select new { p, ff, uu };

            var totalCount = await queryList.CountAsync();
            //根据更新日期排序
            var items = await queryList.OrderByDescending(p => p.ff.CreationTime).PageBy(input).ToListAsync();
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<FormSchemeListShowDto>(item.p);
                dto.Type = item.ff.Type;
                dto.CreatorUserName = item.uu?.Name;
                dto.UpdateTime = item.ff.CreationTime;
                return dto;
            }).ToList();
            return new PagedResultDto<FormSchemeListShowDto>(totalCount, list);
        }

        /// <summary>
        /// 修改表单状态
        /// 表单列表直接更改表单状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateFormSchemeState(UpdateFormSchemeStateDto input)
        {
            var info = await this._formSchemeInfoRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("修改失败：表单记录不存在！");
            }
            await this._formSchemeInfoRepository.UpdateAsync(input.Id, async entity => { entity.IsActive = input.state; });
        }

        /// <summary>
        /// 删除指定的表单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteFormSchemeAsync(EntityDto<Guid> input)
        {
            await _formSchemeInfoRepository.DeleteAsync(input.Id);

        }

        /// <summary>
        /// 获取数据源列名\获取sql列 /custmerform/sql/colnames/lrsystemdb
        /// 表单设计，选择视图表单，录入语句保存后调用接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<string>> GetDataColName(GetDataColNameQueryDto input)
        {
            var vSql = SqlFiltersCommon.SqlFiltersNotSelect(input.sql);
            vSql = vSql.Replace("=@param", " is not null");
            vSql = vSql.Replace("= @param", " is not null");
            return await _dataBaseService.GetSqlColName(vSql);
        }

        /// <summary>
        /// 获取表单历史记录分页列表custmerform/scheme/history/{id}
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<FormSchemeDto>> GetSchemeHistoryPageList(PagedQueryHistoryResultRequestDto input)
        {
            try
            {
                var Scheme = this._formSchemeRepository.GetAll().Where(p => p.SchemeInfoId == input.SchemeInfoId);
                //左链接
                var queryList = from schemeInfo in Scheme
                                join u in _userRepository.GetAll() on schemeInfo.CreatorUserId equals u.Id into f_u
                                from userInfo in f_u.DefaultIfEmpty()
                                select new { schemeInfo, userInfo };

                var totalCount = await queryList.CountAsync();
                var items = await queryList
                    .OrderByDescending(p => p.schemeInfo.CreationTime)
                    .PageBy(input)
                    .ToListAsync();
                var list = items.Select(item =>
                {
                    var dto = ObjectMapper.Map<FormSchemeDto>(item.schemeInfo);
                    dto.CreatorUserName = item.userInfo?.Name;

                    return dto;
                }).ToList();
                return new PagedResultDto<FormSchemeDto>(totalCount, ObjectMapper.Map<List<FormSchemeDto>>(list));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        ///// <summary>
        ///// 获取历史记录预览 custmerform/scheme/history/{id}
        ///// </summary>
        ///// <param name="input">当前记录Id,scheme版本表Id</param>
        ///// <returns></returns>
        //public async Task<FormSchemeDto> GetSchemeHistoryInfo(EntityDto<Guid> input)
        //{
        //    var info = await this._formSchemeRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
        //    return ObjectMapper.Map<FormSchemeDto>(info);
        //}

        /// <summary>
        /// 获取历史记录预览，包含最新版本处理 custmerform/scheme/history/{id}
        /// </summary>
        /// <param name="SchemeIdKey">当前记录Id,scheme版本表Id</param>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task<FormSchemeDto> GetSchemeHistoryInfo(string SchemeIdKey)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new UserFriendlyException(L("DefaultError403"));

            if (string.IsNullOrWhiteSpace(SchemeIdKey))
            {
                throw new UserFriendlyException("参数不能为空");
            }
            //最新版本
            if (SchemeIdKey.IndexOf("lastver") > -1)
            {
                var InfoId = SchemeIdKey.Replace("lastver_", "");
                var schemeInfo = await this._formSchemeRepository.GetAll().Where(p => p.SchemeInfoId.ToString() == InfoId).OrderByDescending(p => p.CreationTime).FirstOrDefaultAsync();
                return ObjectMapper.Map<FormSchemeDto>(schemeInfo);
            }

            var info = await this._formSchemeRepository.GetAll().Where(p => p.Id.ToString() == SchemeIdKey).FirstOrDefaultAsync();
            return ObjectMapper.Map<FormSchemeDto>(info);
        }

        /// <summary>
        /// 更新表单模板版本 custmerform/scheme/history/{id} put
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateSchemeHistoryState(UpdateSchemeHistoryStateDto input)
        {
            var formSchemeEntity = await this._formSchemeRepository.GetAll().Where(p => p.Id == input.SchemeId).FirstOrDefaultAsync();
            FormSchemeInfo entity = new FormSchemeInfo
            {
                Id = input.SchemeInfoId,
                SchemeId = input.SchemeId
            };

            //修改关联模板主键(修改指定字段)
            await this._formSchemeInfoRepository.UpdateAsync(input.SchemeInfoId, async entity => { entity.SchemeId = input.SchemeId; });

        }


        #region 流程表单相关处理接口
        /// <summary>
        /// 获取表单数据-custmerform/data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        [ADTOSharpAllowAnonymous]
        public async Task<Dictionary<string, DataTable>> GetFormDataById(GetFormDataInput input)
        {
            try
            {
                if (!ADTOSharpSession.UserId.HasValue)
                    throw new UserFriendlyException(L("DefaultError403"));
                var schemeEntity = await this.GetSchemeHistoryInfo(input.Id);
                // 需要做兼容处理，来兼容vue2 和 vue3 版本, 两个版本表单数据格式不一致
                List<FormDbTable> list;
                string dbCode;
                FormSchemeModel v3formSchemeModel = schemeEntity.Scheme.ToObject<FormSchemeModel>();
                list = FormHelper.GetQuery(v3formSchemeModel, input.Key, input.KeyValue);
                dbCode = v3formSchemeModel.DbCode;
                var res = new Dictionary<string, DataTable>();
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item.RelationName))
                    {
                        var dt = await _dataBaseService.FindTable(item.Sql);
                        res.Add(item.TableName, dt);
                    }
                    else
                    {
                        if (res[item.RelationName].Rows.Count > 0)
                        {
                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            parameters.Add("keyValue", res[item.RelationName].Rows[0][item.RelationField.ToLower()]);
                            var dt = await _dataBaseService.FindTable(item.Sql, parameters);
                            res.Add(item.TableName, dt);
                        }
                        else
                        {
                            res.Add(item.TableName, new DataTable());
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("操作失败:" + ex.Message);
            }
        }
        /// <summary>
        /// 保存流程表单数据-custmerform/data
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task<string> Save(FormDataDto dto)
        {
            try
            {
                if (!ADTOSharpSession.UserId.HasValue)
                    throw new UserFriendlyException(L("DefaultError403"));

                var schemeEntity = await this.GetSchemeEntity(dto.SchemeId);
                if (schemeEntity == null)
                {
                    throw new UserFriendlyException("找不到表单设计模版！");
                }
                var dataJson = dto.Data.ToJObject();
                // 需要做兼容处理，来兼容vue2 和 vue3 版本, 两个版本表单数据格式不一致
                List<FormDbTable> list;

                string dbCode;
                string historyType;
                //只保留vue3 版本
                FormSchemeModel formSchemeModel = schemeEntity.Scheme.ToObject<FormSchemeModel>();
                list = FormHelper.GetSaveSql(formSchemeModel, dataJson, dto.Pkey, dto.PkeyValue, dto.IsUpdate);
                dbCode = formSchemeModel.DbCode;
                historyType = formSchemeModel.FormInfo.Form.HistoryType;
                if (string.IsNullOrEmpty(list[0].RepeatValueId))
                {
                    foreach (var item in list)
                    {
                        string serviceName = "";
                        if (!string.IsNullOrWhiteSpace(formSchemeModel.MethodName))
                        {
                            serviceName = $"{formSchemeModel.MethodName}.Execute{item.ExecuteType}";
                        }
                        //判断是否服务提交
                        if (!string.IsNullOrWhiteSpace(serviceName) && _invokeMethodHelper.IsInvokeByNameAsync(serviceName))
                        {
                            var parameters = new Dictionary<string, object>();
                            // 遍历参数
                            foreach (var p in item.DbParameter)
                            {
                                parameters.Add($"{p}", p.Value);
                            }
                            if(item.ExecuteType==ExecuteType.Insert||item.ExecuteType==ExecuteType.Update)
                            {
                                var para = ExecuteType.Delete == item.ExecuteType ? dto.PkeyValue : parameters.ToJson();
                                var paradto = new WfMethodDto() { TableName = item.TableName, Data = para };
                                await _invokeMethodHelper.InvokeByNameAsync($"{serviceName}", paradto.ToJson());
                            }else
                            {
                                await _invokeMethodHelper.InvokeByNameAsync($"{serviceName}", dto.PkeyValue);
                            }
                        }
                        else
                        {
                            // 遍历参数
                            foreach (var p in item.DbParameter)
                            {
                                p.Value = p.Value;
                            }
                            var properties = dataJson.Properties();
                            foreach (var item2 in properties)
                            {
                                if (item2.Value.ToString() != null && item2.Value.ToString().IndexOf("designer_code_") != -1)
                                {
                                    dataJson[item2.Name] = (item2.Value.ToString()).ToString();
                                }
                            }
                            switch (item.ExecuteType)
                            {
                                case ExecuteType.Insert:
                                    await ExecuteInsert(item);
                                    break;
                                case ExecuteType.Update:
                                    await ExecuteUpdate(item);
                                    break;
                                case ExecuteType.Delete:
                                    await ExecuteDelete(item);
                                    break;
                            }
                        }
                    }
                }
                return L("保存草稿成功");

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException( ex.Message);
            }
        }
        #region 获取模板的实体
        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<FormScheme> GetSchemeEntity(string keyValue)
        {
            if (keyValue.IndexOf("lastver") > -1)
            {
                var formId = keyValue.Replace("lastver_", "");
                var list = await _formSchemeRepository.GetAll().Where(q => q.SchemeInfoId.Equals(Guid.Parse(formId)) && q.Type == 1).OrderByDescending(d => d.CreationTime).ToListAsync();
                if (list.Count > 0)
                {
                    return list[0];
                }
                return null;
            }
            var id = Guid.Parse(keyValue);
            return await _formSchemeRepository.GetAsync(id);
        }
        #endregion
        #region 保存表单
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="schemeEntity"></param>
        /// <param name="dataJson"></param>
        /// <param name="list"></param>
        /// <param name="dbCode"></param>
        /// <param name="historyType"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task<JObject> SaveFormData(FormDataDto dto, FormScheme schemeEntity, JObject dataJson, List<FormDbTable> list, string dbCode, string historyType)
        {
            try
            {
                foreach (var item in list)
                {
                    // 遍历参数
                    foreach (var p in item.DbParameter)
                    {
                        p.Value = p.Value;
                    }
                    var properties = dataJson.Properties();
                    foreach (var item2 in properties)
                    {
                        if (item2.Value.ToString() != null && item2.Value.ToString().IndexOf("designer_code_") != -1)
                        {
                            dataJson[item2.Name] = (item2.Value.ToString()).ToString();
                        }
                    }
                    switch (item.ExecuteType)
                    {
                        case ExecuteType.Insert:
                            await this.ExecuteInsert(item);
                            break;
                        case ExecuteType.Update:
                            await ExecuteUpdate(item);
                            break;
                        case ExecuteType.Delete:
                            await ExecuteDelete(item);
                            break;
                    }
                }
                return dataJson;
            }
            catch
            {
                throw;
            }

        }
        #endregion
        #region 新增
        private async Task<object> ExecuteInsert(FormDbTable item)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlValue = new StringBuilder();
                sql.Append("INSERT INTO  " + item.TableName + " (");
                sqlValue.Append(" ( ");
                //考虑表字段设计不一样，不写默认字段，完全由表单设计的时候默认传过来
                #region 默认字段
                sql.Append("CreationTime,CreatorUserId,IsDeleted,");
                //sqlValue.Append("@CreationTime,@CreatorUserId,@IsDeleted, ");
                sqlValue.Append("@CreationTime,@CreatorUserId,@IsDeleted,");
                #endregion
                foreach (var v in item.DbParameter)
                {
                    if (v.ParameterName.ToLower() != "CreationTime".ToLower() && v.ParameterName.ToLower() != "CreatorUserId".ToLower() && v.ParameterName.ToLower() != "IsDeleted".ToLower())
                    {
                        //校验是否存在数据
                        sql.Append(v.ParameterName + ",");
                        sqlValue.Append(" @" + v.ParameterName + ",");
                        if (v.ParameterName == "TenantId")
                            parameters.Add("@TenantId", ADTOSharpSession.TenantId);
                        else
                            parameters.Add($"@{v.ParameterName}", v.Value);
                    }
                    //parameters.Add(new SqlParameter(v.ParameterName, v.Value));
                }
                parameters.Add("@CreationTime", DateTime.Now);
                parameters.Add("@CreatorUserId", ADTOSharpSession.UserId);
                //parameters.Add("@TenantId", ADTOSharpSession.TenantId);
                parameters.Add("@IsDeleted", false);

                sql = sql.Remove(sql.Length - 1, 1);
                sqlValue = sqlValue.Remove(sqlValue.Length - 1, 1);
                sql.Append(" ) VALUES " + sqlValue + ");");

                var res = await _dataBaseService.ExecuteSql(sql.ToString(), parameters);
                return res;

            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 编辑
        private async Task<object> ExecuteUpdate(FormDbTable item)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();
                bool isfirt = true;

                sql.Append($"update {item.TableName} set ");
                foreach (var v in item.DbParameter)
                {
                    if (!isfirt)
                        sql.Append(",");
                    //校验是否存在数据
                    sql.Append($"{v.ParameterName}=@{v.ParameterName}");

                    if (v.ParameterName == "TenantId")
                        parameters.Add("@TenantId", ADTOSharpSession.TenantId);
                    else
                        parameters.Add($"@{v.ParameterName}", v.Value);
                    isfirt = false;
                }
                sql.Append(" where 1=1");
                string[] array = item.Pkey.Split(",");
                foreach (string keyItem in array)
                {
                    sql.Append(" AND " + keyItem + " = @" + keyItem + " ");
                    var v = item.DbParameter.Where(q => q.ParameterName == keyItem).FirstOrDefault();
                }

                var res = _dataBaseService.ExecuteSql(sql.ToString(), parameters);
                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
        #region 删除
        private async Task<object> ExecuteDelete(FormDbTable item)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                //List<SqlParameter> parameters = new List<SqlParameter>();
                StringBuilder sql = new StringBuilder();
                #region 注释代码
                //var fields = await _dataBaseService.GetColumnInfosByTableName(item.TableName);
                //var isDeleteField = fields.Where(q => q.DbColumnName.Contains("IsDeleted")).ToList();
                //if (isDeleteField.Count()>0)
                //{
                //    //逻辑删除
                //    sql.Append($"update {item.TableName} set ");
                //    foreach (var field in fields)
                //    {
                //        switch (field.DbColumnName)
                //        {
                //            case "DeletionTime":
                //                sql.Append(",DeletionTime=@DeletionTime");
                //                parameters.Add(new SqlParameter("DeletionTime", DateTime.Now));
                //                break;
                //            case "DeleterUserId":
                //                sql.Append(",DeleterUserId=@DeleterUserId");
                //                parameters.Add(new SqlParameter("DeleterUserId", ADTOSharpSession.GetUserId()));
                //                break;
                //            case "IsDeleted":
                //                sql.Append(",IsDeleted=@IsDeleted");
                //                parameters.Add(new SqlParameter("IsDeleted", false));
                //                break;
                //        }
                //    }
                //    sql.Append(" where id=@id");
                //    parameters.Add(new SqlParameter("id", item.Pkey));
                //}
                //else
                //{  //}
                #endregion
                //物理删除
                sql.Append($"delete {item.TableName} where 1=1");
                foreach (SqlParameter v in item.DbParameter)
                {
                    sql.Append(" AND [" + v.ParameterName + "] = @" + v.ParameterName + " ");
                    parameters.Add($"@{v.ParameterName}", v.Value);
                }
                var res = _dataBaseService.ExecuteSql(sql.ToString(), parameters);
                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
        #endregion

        #region 获取表单数据

        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="processId">流程实例主键值</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<string> GetCustmerFormData(Guid processId, string value)
        {
            var values = value.Split(",");
            var dbCode = values[0];
            var table = values[1];
            var rfield = values[2];
            var field = ""; //values[3];

            for (int i = 3; i < values.Length; i++)
            {
                if (values[i] == "*")
                    field += $"{values[i]},";
                else
                    field += $"[{values[i]}],";
            }
            string strSql = string.Format("select {0} from {1} where [{2}] = @processId ", field.TrimEnd(','), table, rfield);
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("processId", processId);
                DataTable dataTable = await _dataBaseService.FindTable(strSql, parameters);
                if (dataTable.Rows.Count > 0)
                {
                    string filedValues = "";
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        filedValues += $"{dataTable.Rows[0][column.ColumnName.ToLower()].ToString()},";
                    }
                    return filedValues.TrimEnd(',');
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            return "";
        }
        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="processId">流程实例主键值</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<DataTable> GetCustmerFormDataTable(Guid processId, string value)
        {
            try
            {
                var values = value.Split(",");
                var dbCode = values[0];
                var table = values[1];
                var rfield = values[2];
                var field = ""; //values[3];

                for (int i = 3; i < values.Length; i++)
                {
                    if (values[i] == "*")
                        field += $"{values[i]},";
                    else
                        field += $"[{values[i]}],";
                }
                string strSql = string.Format("select {0} from {1} where [{2}] = @processId ", field.TrimEnd(','), table, rfield);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("processId", processId);
                DataTable dataTable = await _dataBaseService.FindTable(strSql, parameters);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="wfScheme"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<string> GetFormKeyword(Guid processId, WFScheme wfScheme)
        {
            string res = "";
            if (!string.IsNullOrWhiteSpace(wfScheme.KeywordDb)
               && !string.IsNullOrWhiteSpace(wfScheme.KeywordTable)
               && !string.IsNullOrWhiteSpace(wfScheme.KeywordRField)
               && !string.IsNullOrWhiteSpace(wfScheme.KeywordSField)
                )
            {

                string[] slist = wfScheme.KeywordSField.Split(",");
                string strsql = "select ";
                foreach (var item in slist)
                {
                    strsql += $"{item},";
                }
                strsql += "from";
                strsql = strsql.Replace(",from", " from");
                strsql += $" {wfScheme.KeywordTable} where {wfScheme.KeywordRField} = @processId ";

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("processId", processId);

                DataTable dt = await _dataBaseService.FindTable(strsql, parameters);
                if (dt.Rows.Count > 0)
                {
                    foreach (var item in slist)
                    {
                        if (!string.IsNullOrEmpty(res))
                        {
                            res += ",";
                        }
                        res += $"{dt.Rows[0][item.ToLower()].ToString()}";
                    }
                }
            }
            return res;
        }
        #endregion
    }
}

