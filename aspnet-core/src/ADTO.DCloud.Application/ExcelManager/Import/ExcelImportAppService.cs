using ADTO.DCloud.CodeRule.Dto;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.ExcelManager.Import.Dto;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;

namespace ADTO.DCloud.ExcelManager.Import
{
    /// <summary>
    /// Excel导入配置相关方法
    /// </summary>
    public class ExcelImportAppService : DCloudAppServiceBase, IExcelImportAppService
    {
        #region
        private readonly IRepository<ExcelImport, Guid> _excelImportRepository;
        private readonly IRepository<ExcelImportField, Guid> _excelImportFieldRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IDataBaseService _dataBaseService;

        public ExcelImportAppService(
              IRepository<ExcelImport, Guid> excelImportRepository
            , IRepository<ExcelImportField, Guid> excelImportFieldRepository
            , IGuidGenerator guidGenerator
            , IDataBaseService dataBaseService

            )
        {
            _excelImportRepository = excelImportRepository;
            _excelImportFieldRepository = excelImportFieldRepository;
            _guidGenerator = guidGenerator;
            _dataBaseService = dataBaseService;
        }
        #endregion

        #region 配置相关接口

        /// <summary>
        /// 添加Excel导入配置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateExcelImportAsync(CreateExcelImportDto input)
        {
            if (input.FieldDtos == null || input.FieldDtos.Count <= 0)
            {
                throw new UserFriendlyException($"字段信息不能为空");
            }
            var isCodeExists = await _excelImportRepository.GetAll().AnyAsync(p => p.Code == input.Code);
            if (isCodeExists)
            {
                throw new UserFriendlyException($"编码【{input.Code}】已存在，请更换！");
            }

            var info = ObjectMapper.Map<ExcelImport>(input);
            //插入主表
            var resultInfo = await _excelImportRepository.InsertAsync(info);
            //插入字段表
            if (resultInfo != null)
            {
                foreach (var item in input.FieldDtos)
                {
                    var FieldDtos = ObjectMapper.Map<ExcelImportField>(item);
                    FieldDtos.ImportId = resultInfo.Id;
                    await _excelImportFieldRepository.InsertAsync(FieldDtos);
                }
            }
        }

        /// <summary>
        /// 修改Excel导入配置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateExcelImportAsync(CreateExcelImportDto input)
        {
            if (input.Id == null || input.Id == Guid.Empty)
            {
                throw new UserFriendlyException($"参数错误，Id为空");
            }
            if (input.FieldDtos == null || input.FieldDtos.Count <= 0)
            {
                throw new UserFriendlyException($"字段信息不能为空");
            }

            var existingConfig = await _excelImportRepository.GetAsync(input.Id.Value);
            if (existingConfig == null)
            {
                throw new UserFriendlyException($"参数错误，Id 不存在");
            }
            var isCodeExists = await _excelImportRepository.GetAll().AnyAsync(p => p.Code == input.Code && p.Id != input.Id.Value);
            if (isCodeExists)
            {
                throw new UserFriendlyException($"编码【{input.Code}】已存在，请更换！");
            }
            ObjectMapper.Map(input, existingConfig);
            await _excelImportRepository.UpdateAsync(existingConfig);

            //批量删除字段表
            await _excelImportFieldRepository.GetAll().Where(p => p.ImportId == input.Id).ExecuteDeleteAsync();

            //插入字段表

            foreach (var item in input.FieldDtos)
            {
                var FieldDtos = ObjectMapper.Map<ExcelImportField>(item);
                FieldDtos.ImportId = input.Id.Value;
                await _excelImportFieldRepository.InsertAsync(FieldDtos);

            }
        }

        /// <summary>
        /// 删除指定的Excel导入配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteExcelImportAsync(EntityDto<Guid> input)
        {
            if (input == null || input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("操作失败，配置ID不能为空！");
            }

            var info = await _excelImportRepository.GetAsync(input.Id);
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            //先删除字段表   
            await _excelImportFieldRepository.GetAll().Where(p => p.ImportId == input.Id).ExecuteDeleteAsync();

            //删除主配置表
            await _excelImportRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取Excel导入配置分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        public async Task<PagedResultDto<ExcelImportDto>> GetExcelImportPageList(PagedExcelImportResultRequestDto input)
        {
            var query = _excelImportRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Name.Contains(input.KeyWord) || p.Code == input.KeyWord);

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;

            return new PagedResultDto<ExcelImportDto>(totalCount, ObjectMapper.Map<List<ExcelImportDto>>(items));
        }

        /// <summary>
        /// 获取导入配置详情，包含关联栏位信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ExcelImportDetailDto> GetExcelImportInfo(EntityDto<Guid> input)
        {
            var info = await this._excelImportRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var infoDto = ObjectMapper.Map<ExcelImportDetailDto>(info);

            var fields = await this._excelImportFieldRepository.GetAll().Where(p => p.ImportId == input.Id).ToListAsync();
            infoDto.FieldDtos = ObjectMapper.Map<List<ExcelImportFieldDto>>(fields);
            return infoDto;
        }

        /// <summary>
        /// 修改配置状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task UpdateExcelImportIsActive(UpdateExcelImportIsActiveDto input)
        {
            if (input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("操作失败，配置ID不能为空！");
            }
            var info = this._excelImportRepository.GetAll().Any(p => p.Id == input.Id);
            if (!info)
            {
                throw new UserFriendlyException("操作失败，当前记录不存在！");
            }

            await this._excelImportRepository.UpdateAsync(input.Id, async entity => { entity.IsActive = input.IsActive; });
        }

        #endregion

        #region 导入操作

        /// <summary>
        /// 执行Excel导入数据库（核心方法）
        /// </summary>
        /// <param name="input">导入参数（配置Code+Excel文件）</param>
        /// <returns>导入结果</returns>
        //[HttpPost]
        [HiddenApi]
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public async Task<ExcelImportResultDto> ExecuteExcelImportAsync([FromForm] ExecuteExcelImportDto input)
        {
            var result = new ExcelImportResultDto();

            try
            {
                // 1. 基础参数校验
                if (input == null)
                {
                    throw new UserFriendlyException("导入参数不能为空");
                }
                if (string.IsNullOrWhiteSpace(input.ImportCode))
                {
                    throw new UserFriendlyException("导入配置Code不能为空");
                }
                //if (input.File == null || input.File.Length == 0)
                //{
                //    throw new UserFriendlyException("请上传有效的Excel文件");
                //}

                if (input.FileContent == null || input.FileContent.Length == 0)
                {
                    throw new UserFriendlyException("请上传有效的Excel文件");
                }

                // 2. 获取导入配置（包含错误处理机制）
                var importConfig = await _excelImportRepository.GetAll().Where(p => p.Code == input.ImportCode).FirstOrDefaultAsync();
                if (importConfig == null || !importConfig.IsActive)
                {
                    throw new UserFriendlyException("导入配置不存在或已禁用");
                }

                // 3. 获取字段映射配置（Excel列名 ↔ 数据库字段名）
                var fieldConfigs = await _excelImportFieldRepository
                    .GetAll()
                    .Where(p => p.ImportId == importConfig.Id)
                    .ToListAsync();
                if (fieldConfigs.Count == 0)
                {
                    throw new UserFriendlyException("该导入配置未配置字段映射关系");
                }

                // 4. 解析Excel文件为DataTable（兼容.xls/.xlsx）
                //var dataTable = await ParseExcelToDataTableAsync(input.File, fieldConfigs);
                var dataTable = await ParseExcelToDataTableAsync(input.FileContent, input.FileName, fieldConfigs);
                if (dataTable.Rows.Count == 0)
                {
                    result.Success = true;
                    result.ErrorMessages.Add("Excel文件中无有效数据");
                    return result;
                }

                // 5. 执行数据导入（含验证+入库）
                var (successCount, failCount, errorMessages) = await ImportDataToDatabaseAsync(
                    dataTable, importConfig, fieldConfigs);

                // 6. 组装返回结果
                result.Success = failCount == 0 || importConfig.ErrorType == 1; // 1=跳过错误行
                result.SuccessCount = successCount;
                result.FailCount = failCount;
                result.ErrorMessages = errorMessages;
            }
            catch (UserFriendlyException ex)
            {
                // 业务异常：友好提示
                result.Success = false;
                result.ErrorMessages.Add(ex.Message);
            }
            catch (Exception ex)
            {
                // 系统异常：记录日志+返回通用提示

                result.Success = false;
                result.ErrorMessages.Add($"导入失败：{ex.Message}");
            }

            return result;
        }

        #region 导入私有辅助方法
        /// <summary>
        /// 解析Excel文件为DataTable（适配字段映射配置）
        /// </summary>
        /// <param name="fileContent">文件字节</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fieldConfigs">字段映射配置</param>
        /// <returns>解析后的数据表</returns>
        //private async Task<DataTable> ParseExcelToDataTableAsync(IFormFile file, List<ExcelImportField> fieldConfigs) 没有控制器层的方式
        private async Task<DataTable> ParseExcelToDataTableAsync(byte[] fileContent, string fileName, List<ExcelImportField> fieldConfigs)
        {
            var dataTable = new DataTable();
            // 构建列：列名=数据库字段名，列别名=Excel列名
            foreach (var config in fieldConfigs)
            {
                dataTable.Columns.Add(config.Name); // 数据库字段名作为列名
            }

            // 读取Excel文件流
            using (var stream = new MemoryStream(fileContent))
            {
                //await file.CopyToAsync(stream);
                //stream.Position = 0;

                IWorkbook workbook = Path.GetExtension(fileName).ToLower() switch
                {
                    ".xlsx" => new XSSFWorkbook(stream),
                    ".xls" => new HSSFWorkbook(stream),
                    _ => throw new UserFriendlyException("仅支持.xls和.xlsx格式的Excel文件")
                };

                // 读取第一个工作表
                var sheet = workbook.GetSheetAt(0);
                if (sheet.LastRowNum == 0)
                {
                    return dataTable; // 无数据
                }

                // 读取表头行（第一行），构建Excel列名→索引的映射
                var headerRow = sheet.GetRow(0);
                var colIndexMap = new Dictionary<string, int>(); // Excel列名 → 列索引
                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    var colName = headerRow.GetCell(i)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(colName))
                    {
                        colIndexMap[colName] = i;
                    }
                }

                // 校验Excel是否包含配置的列
                var missingCols = fieldConfigs
                    .Where(c => !colIndexMap.ContainsKey(c.ColName))
                    .Select(c => c.ColName)
                    .ToList();
                if (missingCols.Any())
                {
                    throw new UserFriendlyException($"Excel缺少必填列：{string.Join(",", missingCols)}");
                }

                // 读取数据行（从第二行开始）
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    var dataRow = dataTable.NewRow();
                    bool isEmptyRow = true;

                    // 按字段配置映射数据
                    foreach (var config in fieldConfigs)
                    {
                        var cell = row.GetCell(colIndexMap[config.ColName]);
                        object cellValue = DBNull.Value;

                        try
                        {
                            if (cell == null)
                            {
                                cellValue = DBNull.Value;
                            }
                            else if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                // ✅ 日期统一转成 SQL 能识别的标准格式
                                cellValue = cell.DateCellValue.HasValue
                                    ? cell.DateCellValue.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                    : DBNull.Value;
                            }
                            else
                            {
                                cellValue = cell.ToString()?.Trim() ?? "";
                                if (string.IsNullOrWhiteSpace(cellValue.ToString()))
                                    cellValue = DBNull.Value;
                            }
                        }
                        catch
                        {
                            cellValue = DBNull.Value;
                        }

                        // 赋值到DataRow
                        if (cellValue != DBNull.Value && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                        {
                            isEmptyRow = false;
                        }
                        dataRow[config.Name] = cellValue;
                    }

                    // 跳过空行
                    if (!isEmptyRow)
                    {
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;
        }

        /// <summary>
        /// 将解析后的数据导入数据库（含唯一性验证+错误处理）
        /// </summary>
        /// <param name="dataTable">解析后的Excel数据</param>
        /// <param name="importConfig">导入配置</param>
        /// <param name="fieldConfigs">字段映射配置</param>
        /// <returns>成功数、失败数、错误信息</returns>
        /// 
        private async Task<(int successCount, int failCount, List<string> errorMessages)> ImportDataToDatabaseAsync(DataTable dataTable, ExcelImport importConfig, List<ExcelImportField> fieldConfigs)
        {
            int successCount = 0;
            int failCount = 0;
            var errorMessages = new List<string>();
            var errorType = importConfig.ErrorType ?? 0; // 0=终止，1=跳过
            //查询目标表的实际字段（避免添加不存在的列
            var tableColumns = await _dataBaseService.GetColumnInfosByTableName(importConfig.DbTable);
            var tableColumnNames = tableColumns.Select(p => p.DbColumnName).ToList(); // 提取字段名列表
            var systemFields = new Dictionary<string, object>()
            {
                //{ "CreationTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }, // 创建时间：当前时间
                { "CreationTime", DateTime.Now}, // 创建时间：当前时间
                { "CreatorUserId",   ADTOSharpSession.UserId ?? Guid.Empty }, // 创建人ID：当前登录用户 
                { "IsDeleted", false }, // 软删除标记：默认false
               
            };

            // 2. 为DataTable补充系统字段列（若不存在）
            foreach (var sysField in systemFields)
            {
                string sysFieldName = sysField.Key;
                // 只处理表中存在的系统字段
                if (tableColumnNames.Contains(sysFieldName, StringComparer.OrdinalIgnoreCase))
                {
                    // 补充列到DataTable
                    if (!dataTable.Columns.Contains(sysFieldName))
                    {
                        Type colType = sysField.Key switch
                        {
                            "CreationTime" => typeof(DateTime),    // 日期类型
                            "CreatorUserId" => typeof(Guid),       // Guid类型
                            "IsDeleted" => typeof(bool),           // 布尔类型
                            "Id" => typeof(Guid),                  // Guid类型
                            _ => typeof(object)                    // 其他字段默认
                        };
                        dataTable.Columns.Add(sysFieldName, colType);
                    }

                    // 为每行赋值
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row[sysFieldName] == DBNull.Value || row[sysFieldName] == null || string.IsNullOrEmpty(row[sysFieldName].ToString()))
                        {
                            row[sysFieldName] = sysField.Value; // 执行延迟赋值
                        }
                    }

                    // 关键：将系统字段加入fieldConfigs（确保BuildInsertSql能遍历到）
                    if (!fieldConfigs.Any(f => f.Name.Equals(sysFieldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        fieldConfigs.Add(new ExcelImportField { Name = sysFieldName });
                    }
                }
            }
            var primaryKeyField = fieldConfigs.FirstOrDefault(f => f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            bool isPrimaryKeyInConfig = primaryKeyField != null;
            bool tableHasIdColumn = tableColumnNames.Contains("Id", StringComparer.OrdinalIgnoreCase);
            if (!isPrimaryKeyInConfig && tableHasIdColumn)
            {
                if (!dataTable.Columns.Contains("Id"))
                {
                    dataTable.Columns.Add("Id", typeof(Guid));
                }
                // 只添加一次，避免重复
                if (!fieldConfigs.Any(f => f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
                {
                    fieldConfigs.Add(new ExcelImportField { Name = "Id" });
                }
            }

            // 获取数据库连接（适配ABP的多数据库配置，这里以SQL Server为例）
            var dbContext = _excelImportRepository.GetDbContext();
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    try
                    {
                        // 行号（Excel中从第二行开始，所以+1）
                        var rowNum = dataTable.Rows.IndexOf(row) + 2;

                        if (tableHasIdColumn)
                        {
                            row["Id"] = _guidGenerator.Create();
                        }

                        // 1. 唯一性验证（根据ExcelImportField的IsOnlyOne配置）
                        await ValidateDataUniquenessAsync(row, importConfig.DbTable, fieldConfigs, rowNum);

                        //数据格式化
                        await ConvertExcelValueAsync(row, fieldConfigs, rowNum);

                        // 2. 构建插入SQL
                        var (columns, parameters, paramValues) = BuildInsertSql(row, fieldConfigs);
                        var sql = $"INSERT INTO {importConfig.DbTable} ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";

                        // 3. 执行插入
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = sql;
                            //command.Transaction = (System.Data.Common.DbTransaction)dbContext.Database.CurrentTransaction?.GetDbTransaction();

                            // 添加参数（防止SQL注入）
                            foreach (var (paramName, value) in paramValues)
                            {
                                var param = command.CreateParameter();
                                param.ParameterName = paramName;
                                param.Value = value ?? DBNull.Value;
                                command.Parameters.Add(param);
                            }

                            await command.ExecuteNonQueryAsync();
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        var errorMsg = $"第{dataTable.Rows.IndexOf(row) + 2}行导入失败：{ex.Message}";
                        errorMessages.Add(errorMsg);

                        // 错误处理机制：0=终止（抛出异常），1=跳过（继续处理下一行）
                        if (errorType == 0)
                        {
                            throw new Exception(errorMsg, ex);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            return (successCount, failCount, errorMessages);
        }

        /// <summary>
        /// 验证数据唯一性（根据IsOnlyOne配置）
        /// </summary>
        private async Task ValidateDataUniquenessAsync(DataRow row, string dbTable, List<ExcelImportField> fieldConfigs, int rowNum)
        {
            // 筛选需要唯一性验证的字段
            var uniqueFields = fieldConfigs.Where(c => c.IsOnlyOne == true).ToList();
            if (!uniqueFields.Any()) return;

            var dbContext = _excelImportRepository.GetDbContext();
            var connection = dbContext.Database.GetDbConnection();

            foreach (var field in uniqueFields)
            {
                var fieldValue = row[field.Name];
                if (fieldValue == DBNull.Value || string.IsNullOrEmpty(fieldValue.ToString()))
                {
                    throw new UserFriendlyException($"字段【{field.ColName}】为唯一校验字段，不能为空");
                }

                // 构建唯一性校验SQL
                var sql = $"SELECT COUNT(1) FROM {dbTable} WHERE {field.Name} = @{field.Name}";
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    var param = command.CreateParameter();
                    param.ParameterName = $"@{field.Name}";
                    param.Value = fieldValue;
                    command.Parameters.Add(param);

                    var count = (int)await command.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        throw new UserFriendlyException($"字段【{field.ColName}】的值「{fieldValue}」已存在，违反唯一性约束");
                    }
                }
            }

        }

        /// <summary>
        /// 构建插入SQL和参数（防止SQL注入）
        /// </summary>
        private (List<string> columns, List<string> parameters, Dictionary<string, object> paramValues) BuildInsertSql(
            DataRow row, List<ExcelImportField> fieldConfigs)
        {
            var columns = new List<string>();
            var parameters = new List<string>();
            var paramValues = new Dictionary<string, object>();

            foreach (var config in fieldConfigs)
            {
                var columnName = config.Name;
                if (!row.Table.Columns.Contains(columnName))
                    continue;

                var paramName = $"@{columnName}";
                var value = row[columnName];

                columns.Add(columnName);
                parameters.Add(paramName);
                paramValues.Add(paramName, value);
            }

            return (columns, parameters, paramValues);
        }


        /// <summary>
        /// 统一转换：字典 + 动态外键（任意表、任意字段）
        /// </summary>
        private async Task ConvertExcelValueAsync(DataRow row, List<ExcelImportField> fieldConfigs, int rowNum)
        {
            foreach (var field in fieldConfigs)
            {
                if (field.ConvertType == 0) continue;

                string excelValue = row[field.Name]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(excelValue)) continue;

                try
                {
                    // 字典转换
                    if (field.ConvertType == 1)
                    {
                        var dbContext = _excelImportRepository.GetDbContext();
                        var conn = dbContext.Database.GetDbConnection();
                        bool wasClosed = conn.State == ConnectionState.Closed;
                        if (wasClosed) await conn.OpenAsync();
                        var sql = @"SELECT t1.Id,t1.ItemName,T1.ItemValue FROM DataItemDetails AS t1 LEFT JOIN DataItems AS t2 ON T1.ItemId = t2.Id WHERE t2.ItemCode = @type AND t1.ItemName = @label AND t1.IsDeleted = 0";
                        //var sql = @"SELECT TOP 1 DictValue FROM DataItemDetails WHERE DictTypeCode = @type AND DictLabel = @label AND IsDeleted=0";
                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = sql;
                        var p1 = cmd.CreateParameter(); p1.ParameterName = "@type"; p1.Value = field.DictTypeCode; cmd.Parameters.Add(p1);
                        var p2 = cmd.CreateParameter(); p2.ParameterName = "@label"; p2.Value = excelValue; cmd.Parameters.Add(p2);
                        var dictValue = await cmd.ExecuteScalarAsync();
                        if (dictValue == null || dictValue == DBNull.Value) throw new Exception($"字典项「{excelValue}」不存在");
                        row[field.Name] = dictValue;

                        if (wasClosed) await conn.CloseAsync();
                    }

                    // 动态外键转换（任意表、任意字段）
                    if (field.ConvertType == 2)
                    {
                        if (string.IsNullOrWhiteSpace(field.RelationEntity) || string.IsNullOrWhiteSpace(field.MatchField))
                            throw new Exception("未配置关联表/匹配字段");

                        var dbContext = _excelImportRepository.GetDbContext();
                        var conn = dbContext.Database.GetDbConnection();
                        bool wasClosed = conn.State == ConnectionState.Closed;
                        if (wasClosed) await conn.OpenAsync();

                        string table = field.RelationEntity.Replace("'", "").Replace(";", "");
                        string fieldName = field.MatchField.Replace("'", "").Replace(";", "");
                        string sql = $@"SELECT TOP 1 Id FROM {table} WHERE {fieldName} = @v  AND IsDeleted=0";

                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = sql;
                        var p1 = cmd.CreateParameter(); p1.ParameterName = "@v"; p1.Value = excelValue; cmd.Parameters.Add(p1);
                        // var p2 = cmd.CreateParameter(); p2.ParameterName = "@tid"; p2.Value = ABPSession.TenantId ?? (object)DBNull.Value; cmd.Parameters.Add(p2);

                        var id = await cmd.ExecuteScalarAsync();

                        if (id == null || id == DBNull.Value) throw new Exception($"在【{field.RelationEntity}】中未找到：{excelValue}");
                        row[field.Name] = (Guid)id;

                        if (wasClosed) await conn.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException($"第{rowNum}行【{field.ColName}】转换失败：{ex.Message}");
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 导出指定编码的Excel导入模板
        /// </summary>
        /// <param name="ImportCode">配置编码</param>
        /// <returns></returns>
        //[HttpGet]
        [HiddenApi]
        public async Task<byte[]> DownloadImportTemplateAsync(string ImportCode)
        {
            if (string.IsNullOrWhiteSpace(ImportCode))
                throw new UserFriendlyException("导入配置Code不能为空");

            // 获取配置
            var importConfig = await _excelImportRepository.GetAll()
                .FirstOrDefaultAsync(p => p.Code == ImportCode);

            if (importConfig == null)
                throw new UserFriendlyException("导入配置不存在");

            // 获取字段
            var fieldConfigs = await _excelImportFieldRepository.GetAll()
                .Where(p => p.ImportId == importConfig.Id)
                .ToListAsync();

            if (!fieldConfigs.Any())
                throw new UserFriendlyException("该配置未设置导入字段");

            // 创建Excel
            byte[] excelBytes;

            using (var ms = new MemoryStream())
            {
                using (var workbook = new XSSFWorkbook())
                {
                    var sheet = workbook.CreateSheet(importConfig.Name);
                    var headerRow = sheet.CreateRow(0);

                    for (int i = 0; i < fieldConfigs.Count; i++)
                    {
                        headerRow.CreateCell(i).SetCellValue(fieldConfigs[i].ColName);
                        sheet.AutoSizeColumn(i);
                    }

                    workbook.Write(ms);
                    excelBytes = ms.ToArray();
                }
            }

            return excelBytes;
            //return new FileContentResult(
            //         excelBytes,
            //         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            //     )
            //            {
            //    FileDownloadName = $"{importConfig.Name}_导入模板.xlsx"
            //};
        }
    }

}



