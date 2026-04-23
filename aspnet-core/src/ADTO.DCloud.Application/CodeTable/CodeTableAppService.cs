using System;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using ADTO.DCloud.CodeTable.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.DataBase.Model;
using System.Collections.Generic;
using ADTO.DCloud.DataBase;
using ADTOSharp.UI;
using NuGet.Packaging;
using ADTOSharp.EntityFrameworkCore.Repositories;

namespace ADTO.DCloud.CodeTable
{
    /// <summary>
    /// 数据库表信息、字段信息相关操作
    /// </summary>
    public class CodeTableAppService : DCloudAppServiceBase, ICodeTableAppService
    {
        private readonly IRepository<CodeTable, Guid> _codeTableRepository;
        private readonly IRepository<CodeColumns, Guid> _codeColumnsRepository;
        private readonly IDataBaseService _dataBaseService;
        public CodeTableAppService(IRepository<CodeTable, Guid> codeTableRepository, IRepository<CodeColumns, Guid> codeColumnsRepository, IDataBaseService dataBaseService)
        {
            _codeTableRepository = codeTableRepository;
            _dataBaseService = dataBaseService;
            _codeColumnsRepository = codeColumnsRepository;
        }

        /// <summary>
        /// 查看数据库所有表【导入表列表、】
        /// 做调整，导入表的源表，需要从ImportTableSources表获取，不能直接选择数据库所有表
        /// </summary>
        /// <returns></returns>
        public async Task<List<DbTableInfo>> GetImportTableList()
        {
            try
            {
                return await _dataBaseService.GetTableInfoList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 获取表信息分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CodeTableDto>> GetCodeTablePageList(PagedCodeTabelResultRequestDto input)
        {
            var query = _codeTableRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.ClassName.Contains(input.KeyWord) || p.TableName.Contains(input.KeyWord) || p.Remark.Contains(input.KeyWord));

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<CodeTableDto>(item);
                return dto;
            }).ToList();
            return new PagedResultDto<CodeTableDto>(totalCount, list);
        }

        /// <summary>
        /// 添加信息【导入表，加入新的表数据】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ImportTable(CreateCodeTableInputDto input)
        {
            var oldTables = await this._codeTableRepository.GetAll().ToListAsync();

            List<CodeColumns> updateList = new List<CodeColumns>();
            List<CodeColumns> addList = new List<CodeColumns>();
            List<CodeColumns> deleteList = new List<CodeColumns>();
            foreach (var tableItem in input.TableList)
            {
                var entity = oldTables.FirstOrDefault(t => t.TableName == tableItem.Name);
                var dbColumns = await _dataBaseService.GetColumnInfosByTableName(tableItem.Name);
                int columnSort = 1;

                if (entity == null)
                {
                    // 新增
                    CodeTable insertDto = new CodeTable
                    {
                        DbId = "", //数据库关键词 默认lrsystemdb
                        ClassName = CsharpName(tableItem.Name),
                        TableName = tableItem.Name,
                        Remark = tableItem.Description,
                        IsLock = 0,
                        State = 1,
                    };
                    var codeTableId = await _codeTableRepository.InsertAndGetIdAsync(insertDto);

                    // 新增列
                    foreach (var dbColumnItem in dbColumns)
                    {
                        var codeColumns = CreateColumn(dbColumnItem);
                        codeColumns.CodeTableId = codeTableId;
                        codeColumns.DisplayOrder = columnSort++;
                        addList.Add(codeColumns);
                    }
                }
                else
                {
                    // 编辑
                    if (entity.Remark != tableItem.Description || entity.State != 1 || entity.IsLock != 0 || entity.IsDeleted != false)
                    {
                        entity.State = 1;
                        entity.Remark = tableItem.Description;
                        entity.IsLock = 0;
                        entity.IsDeleted = false;

                        await this._codeTableRepository.UpdateAsync(entity);
                    }
                    var oldColumns = await _codeColumnsRepository.GetAll().Where(p => p.CodeTableId == entity.Id).AsNoTracking().ToListAsync();
                    foreach (var dbColumnItem in dbColumns)
                    {
                        var codeColumnsEntity = CreateColumn(dbColumnItem);
                        codeColumnsEntity.CodeTableId = entity.Id;
                        codeColumnsEntity.DisplayOrder = columnSort;
                        columnSort++;

                        var existingEntity = oldColumns.FirstOrDefault(t => t.DbColumnName == dbColumnItem.DbColumnName);
                        if (existingEntity == null)
                        {
                            addList.Add(codeColumnsEntity);
                        }
                        else
                        {
                            codeColumnsEntity.Id = existingEntity.Id;
                            updateList.Add(codeColumnsEntity);
                            //直接修改，暂时不批量修改
                            await this._codeColumnsRepository.UpdateAsync(codeColumnsEntity);
                        }
                    }
                    // 获取需要删除的列
                    var deleteColumns = oldColumns.FindAll(t => updateList.FindIndex(t2 => t2.Id == t.Id) == -1);
                    deleteList.AddRange(deleteColumns);
                }
            }
            
            await this._codeColumnsRepository.InsertRangeAsync(addList);
            // await this._codeColumnsRepository.BatchUpdateAsync(updateList);

            _codeColumnsRepository.RemoveRange(deleteList);
        }

        /// <summary>
        /// 选择表获取表及字段信息
        /// 对应老接口data/codeTables/{dbCode}
        /// </summary>
        /// <param name="tableNames">选中的表名，用逗号分隔</param>
        /// <returns></returns>
        public async Task<List<CodeTableDetailInfoDto>> GetCodeTableDetail(string tableNames)
        {
            if (string.IsNullOrWhiteSpace(tableNames))
            {
                return new List<CodeTableDetailInfoDto>();
            }
            var nameList = tableNames.Split(",").ToList();

            var tableList = await this._codeTableRepository.GetAll().Where(p => nameList.Contains(p.TableName)).ToListAsync();

            // 通过单个查询获取所有相关的 CodeColumns，以提高性能
            var codeColumnsLookup = (await this._codeColumnsRepository.GetAll()
                .Where(p => tableList.Select(t => t.Id).Contains(p.CodeTableId))
                .ToListAsync())
                .GroupBy(cc => cc.CodeTableId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var infoDtos = tableList.Select(item => new CodeTableDetailInfoDto
            {
                CodeTableInfo = ObjectMapper.Map<CodeTableDto>(item),
                CodeColumnsList = ObjectMapper.Map<List<CodeColumnsDto>>(codeColumnsLookup.ContainsKey(item.Id) ? codeColumnsLookup[item.Id] : new List<CodeColumns>())
            }).ToList();

            #region 优化前代码
            //foreach (var item in tableList)
            //{
            //    CodeTableDetailInfoDto detailInfoDto = new CodeTableDetailInfoDto();
            //    detailInfoDto.CodeTableInfo = ObjectMapper.Map<CodeTableDto>(item);

            //    detailInfoDto.CodeColumnsList = ObjectMapper.Map<List<CodeColumnsDto>>(await this._codeColumnsRepository.GetAll().Where(p => p.CodeTableId == item.Id).ToListAsync());
            //    infoDtos.Add(detailInfoDto);

            //}
            #endregion
            return infoDtos;
        }


        #region 扩展方法
        /// <summary>
        /// 获取类字段信息
        /// </summary>
        /// <param name="dbColumnInfo">数据库字段信息</param>
        /// <returns></returns>
        private CodeColumns CreateColumn(DbColumnInfo dbColumnInfo)
        {
            var codeColumnsEntity = new CodeColumns();
            codeColumnsEntity.DbColumnName = dbColumnInfo.DbColumnName;
            codeColumnsEntity.IsNullable = dbColumnInfo.IsNullable == true ? 1 : 0;
            codeColumnsEntity.IsIdentity = dbColumnInfo.IsIdentity == true ? 1 : 0;
            codeColumnsEntity.IsPrimaryKey = dbColumnInfo.IsPrimarykey == true ? 1 : 0;
            codeColumnsEntity.Remark = dbColumnInfo.ColumnDescription;

            codeColumnsEntity.CsType = dbColumnInfo.PropertyName;
            codeColumnsEntity.DbType = dbColumnInfo.DataType;
            codeColumnsEntity.Length = dbColumnInfo.Length;
            codeColumnsEntity.DecimalDigits = dbColumnInfo.DecimalDigits;
            return codeColumnsEntity;
        }

        public static string CsharpName(string dbColumnName)
        {
            if (dbColumnName.Contains("_"))
            {
                string[] value = (from it in dbColumnName.Split('_')
                                  select FirstUpper(it)).ToArray();
                return string.Join("", value);
            }

            return FirstUpper(dbColumnName);
        }

        public static string FirstUpper(string value)
        {
            string text = value.Substring(0, 1).ToUpper();
            return text + value.Substring(1, value.Length - 1);
        }

        #endregion
    }
}

