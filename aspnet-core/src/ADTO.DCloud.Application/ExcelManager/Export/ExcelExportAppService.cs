using ADTO.DCloud.DataBase;
using ADTO.DCloud.ExcelManager.Export.Dto;
using ADTO.DCloud.ExcelManager.Import.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export
{
    /// <summary>
    /// Excel导出配置
    /// </summary>
    public class ExcelExportAppService : DCloudAppServiceBase, IExcelExportAppService
    {
        #region
        private readonly IRepository<ExcelExport, Guid> _excelExportRepository;
        private readonly IRepository<ExcelExportField, Guid> _excelExportFieldRepository;
        private readonly IRepository<ExcelExportParam, Guid> _excelExportParamRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IDataBaseService _dataBaseService;

        public ExcelExportAppService(
              IRepository<ExcelExport, Guid> excelExportRepository
            , IRepository<ExcelExportField, Guid> excelExportFieldRepository
            , IRepository<ExcelExportParam, Guid> excelExportParamRepository
            , IGuidGenerator guidGenerator
            , IDataBaseService dataBaseService

            )
        {
            _excelExportRepository = excelExportRepository;
            _excelExportFieldRepository = excelExportFieldRepository;
            _excelExportParamRepository = excelExportParamRepository;
            _guidGenerator = guidGenerator;
            _dataBaseService = dataBaseService;
        }
        #endregion

        #region 基本方法、增删改查
        /// <summary>
        /// 获取Excel导出配置分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        public async Task<PagedResultDto<ExcelExportsDto>> GetExcelExportPageList(PagedExcelExportResultRequestDto input)
        {
            var query = _excelExportRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Name.Contains(input.KeyWord) || p.Code == input.KeyWord);

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.CreationTime).PageBy(input).ToListAsync(); ;

            return new PagedResultDto<ExcelExportsDto>(totalCount, ObjectMapper.Map<List<ExcelExportsDto>>(items));
        }

        /// <summary>
        /// 获取详情，包含字段、参数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<ExcelExportsDetailDto> GetExcelExportInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _excelExportRepository.GetAsync(input.Id);
            var infoDto = ObjectMapper.Map<ExcelExportsDetailDto>(entity);

            var fields = await this._excelExportFieldRepository.GetAll().Where(p => p.ExportId == input.Id).ToListAsync();
            infoDto.FieldDtos = ObjectMapper.Map<List<ExcelExportFieldDto>>(fields);

            //var parms = await this._excelExportParamRepository.GetAll().Where(p => p.ExportId == input.Id).ToListAsync();
            //infoDto.ParamsDtos = ObjectMapper.Map<List<ExcelExportParamsDto>>(parms);

            return infoDto;
        }

        /// <summary>
        /// 新增Excel 导出配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task CreateExcelExportsAsync(CreateExcelExportsDto input)
        {
            if (input.FieldDtos == null || input.FieldDtos.Count <= 0)
            {
                throw new UserFriendlyException($"字段信息不能为空");
            }
            var isCodeExists = await _excelExportRepository.GetAll().AnyAsync(p => p.Code == input.Code);
            if (isCodeExists)
            {
                throw new UserFriendlyException($"编码【{input.Code}】已存在，请更换！");
            }

            var info = ObjectMapper.Map<ExcelExport>(input);
            //插入主表
            var resultInfo = await _excelExportRepository.InsertAsync(info);
            //插入字段表
            if (resultInfo != null)
            {
                foreach (var item in input.FieldDtos)
                {
                    var FieldDtos = ObjectMapper.Map<ExcelExportField>(item);
                    FieldDtos.ExportId = resultInfo.Id;
                    await _excelExportFieldRepository.InsertAsync(FieldDtos);
                }
            }
            ////插入参数表
            //if (resultInfo != null)
            //{
            //    foreach (var item in input.ParamsDtos)
            //    {
            //        var paramDtos = ObjectMapper.Map<ExcelExportParam>(item);
            //        paramDtos.ExportId = resultInfo.Id;
            //        await _excelExportParamRepository.InsertAsync(paramDtos);
            //    }
            //}
        }

        /// <summary>
        /// 修改Excel 导出配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>

        public async Task UpdateExcelExportAsync(CreateExcelExportsDto input)
        {
            if (input.Id == null || input.Id == Guid.Empty)
            {
                throw new UserFriendlyException($"参数错误，Id为空");
            }
            if (input.FieldDtos == null || input.FieldDtos.Count <= 0)
            {
                throw new UserFriendlyException($"字段信息不能为空");
            }

            var existingConfig = await _excelExportRepository.GetAsync(input.Id.Value);
            if (existingConfig == null)
            {
                throw new UserFriendlyException($"参数错误，Id 不存在");
            }
            var isCodeExists = await _excelExportRepository.GetAll().AnyAsync(p => p.Code == input.Code && p.Id != input.Id.Value);
            if (isCodeExists)
            {
                throw new UserFriendlyException($"编码【{input.Code}】已存在，请更换！");
            }
            ObjectMapper.Map(input, existingConfig);
            await _excelExportRepository.UpdateAsync(existingConfig);

            //批量删除字段表
            await _excelExportFieldRepository.GetAll().Where(p => p.ExportId == input.Id).ExecuteDeleteAsync();

            //插入字段表
            foreach (var item in input.FieldDtos)
            {
                var fieldDtos = ObjectMapper.Map<ExcelExportField>(item);
                fieldDtos.ExportId = input.Id.Value;
                await _excelExportFieldRepository.InsertAsync(fieldDtos);

            }

            ////批量删除参数表
            //await _excelExportParamRepository.GetAll().Where(p => p.ExportId == input.Id).ExecuteDeleteAsync();

            ////插入参数表
            //foreach (var item in input.ParamsDtos)
            //{
            //    var paramDto = ObjectMapper.Map<ExcelExportParam>(item);
            //    paramDto.ExportId = input.Id.Value;
            //    await _excelExportParamRepository.InsertAsync(paramDto);

            //}
        }

        /// <summary>
        /// 删除指定的Excel导入配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteExcelExportAsync(EntityDto<Guid> input)
        {
            if (input == null || input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("操作失败，配置ID不能为空！");
            }

            var info = await _excelExportRepository.GetAsync(input.Id);
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            //先删除字段表   
            await _excelExportFieldRepository.GetAll().Where(p => p.ExportId == input.Id).ExecuteDeleteAsync();
            //删除参数表
            // await _excelExportParamRepository.GetAll().Where(p => p.ExportId == input.Id).ExecuteDeleteAsync();

            //删除主配置表
            await _excelExportRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 修改配置启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task UpdateExcelExportIsActive(UpdateExcelExportIsActiveDto input)
        {
            if (input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("操作失败，配置ID不能为空！");
            }
            var info = this._excelExportRepository.GetAll().Any(p => p.Id == input.Id);
            if (!info)
            {
                throw new UserFriendlyException("操作失败，当前记录不存在！");
            }

            await this._excelExportRepository.UpdateAsync(input.Id, async entity => { entity.IsActive = input.IsActive; });
        }

        #endregion

        #region 数据导出

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="exportCode"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [HiddenApi]
        public async Task<ExcelFileDto> ExecuteExportDataAsync(string exportCode, Dictionary<string, string> queryParams)
        {
            // 1. 获取配置
            var export = await _excelExportRepository.GetAll().Where(p => p.Code == exportCode).FirstOrDefaultAsync();
            if (export == null)
                throw new UserFriendlyException("导出配置不存在");

            // 2. 获取字段
            var fields = await _excelExportFieldRepository.GetAll()
                .Where(x => x.ExportId == export.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            if (!fields.Any())
                throw new UserFriendlyException("未配置导出字段");

            // 3. 获取数据（反射调用业务方法）
            var dataList = await InvokeBusinessMethod(export, queryParams);

            // 4. 格式化数据
            var formattedList = FormatData(dataList, fields);

            // 5. 生成 Excel
            var bytes = BuildExcel(fields, formattedList);

            return new ExcelFileDto(bytes, $"{export.Name}.xlsx");
        }

        /// <summary>
        /// 反射调用业务服务方法
        /// </summary>
        private async Task<List<object>> InvokeBusinessMethod(ExcelExport export, Dictionary<string, string> paramDic)
        {
            Type? serviceType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                serviceType = assembly.GetType(export.ServiceFullName);
                if (serviceType != null)
                    break;
            }
            if (serviceType == null)
                throw new UserFriendlyException("找不到服务：" + export.ServiceFullName);

            var service = IocManager.Instance.Resolve(serviceType);
            var method = serviceType.GetMethod(export.MethodName);
            if (method == null)
                throw new UserFriendlyException("找不到方法：" + export.MethodName);

            var parameters = method.GetParameters();
            var args = new List<object>();

            foreach (var paramInfo in parameters)
            {
                var paramType = paramInfo.ParameterType;

                if (paramType.IsClass && paramType != typeof(string))
                {
                    var dto = Activator.CreateInstance(paramType);

                    foreach (var prop in paramType.GetProperties())
                    {
                        object val = null;
                        string propName = prop.Name;

                        if (paramDic.ContainsKey(propName))
                            val = paramDic[propName];
                        else if (paramDic.ContainsKey(propName.ToLower()))
                            val = paramDic[propName.ToLower()];
                        else if (paramDic.ContainsKey(propName.First().ToString().ToLower() + propName.Substring(1)))
                            val = paramDic[propName.First().ToString().ToLower() + propName.Substring(1)];

                        // 有值就赋值，不玩任何类型转换，100%成功
                        if (val != null)
                        {
                            //prop.SetValue(dto, val);

                            try
                            {
                                // 关键：自动转成目标属性类型
                                //var targetType = prop.PropertyType;
                                //var safeValue = Convert.ChangeType(val, targetType);
                                //prop.SetValue(dto, safeValue);

                                var safeValue = ConvertToType(val.ToString(), prop.PropertyType);
                                prop.SetValue(dto, safeValue);
                            }
                            catch (Exception ex)
                            {
                                throw new UserFriendlyException($"赋值失败：属性 {prop.Name}，值 {val}，错误：{ex.Message}");
                            }
                        }
                    }

                    args.Add(dto);
                }
                else
                {
                    // 普通简单类型
                    if (paramDic.TryGetValue(paramInfo.Name, out var val))
                    {
                        args.Add(Convert.ChangeType(val, paramInfo.ParameterType));
                    }
                    else
                    {
                        args.Add(paramInfo.DefaultValue);
                    }
                }
            }

            var result = method.Invoke(service, args.ToArray());
            var task = result as Task;
            await task;

            var resultType = task.GetType();
            var resultProp = resultType.GetProperty("Result");
            var returnValue = resultProp.GetValue(task);

            object dataList = null;
            var itemsProperty = returnValue.GetType().GetProperty("Items");
            if (itemsProperty != null)
            {
                // 如果是分页Dto → 取 Items
                dataList = itemsProperty.GetValue(returnValue);
            }
            else
            {
                // 如果是直接返回 List/Enumerable
                dataList = returnValue;
            }
            return ((IEnumerable<object>)dataList).ToList();

            //var resultProperty = task.GetType().GetProperty("Result");
            //var list = resultProperty.GetValue(task);

            //return ((IEnumerable<object>)list).ToList();
        }

        private object ConvertToType(string value, Type targetType)
        {
            // 处理可空类型 int? DateTime?
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // 空字符串 → null
            if (string.IsNullOrEmpty(value) && underlyingType != targetType)
                return null!;

            // 处理枚举
            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, value, true);

            // 处理 Guid 
            if (underlyingType == typeof(Guid))
                return Guid.Parse(value);


            // 普通类型转换
            return Convert.ChangeType(value, underlyingType);
        }

        /// <summary>
        /// 格式化日期、布尔
        /// </summary>
        private List<Dictionary<string, object>> FormatData(List<object> dataList, List<ExcelExportField> fields)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (var item in dataList)
            {
                var dic = new Dictionary<string, object>();
                var type = item.GetType();

                foreach (var field in fields)
                {
                    var val = type.GetProperty(field.Name)?.GetValue(item);

                    if (field.FormatType == 1) // 日期
                    {
                        if (val != null && DateTime.TryParse(val.ToString(), out var dt))
                        {
                            val = dt.ToString(field.FormatConfig ?? "yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    else if (field.FormatType == 2) // 布尔
                    {
                        var arr = field.FormatConfig?.Split('、') ?? new[] { "是", "否" };
                        val = val is true ? arr[0] : arr[1];
                    }

                    dic[field.Name] = val;
                }

                result.Add(dic);
            }

            return result;
        }

        /// <summary>
        /// NPOI生成Excel
        /// </summary>
        private byte[] BuildExcel(List<ExcelExportField> fields, List<Dictionary<string, object>> data)
        {
            IWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet("Sheet1");

            // 表头
            IRow header = sheet.CreateRow(0);
            for (int i = 0; i < fields.Count; i++)
            {
                header.CreateCell(i).SetCellValue(fields[i].ColName);
            }

            // 内容
            for (int i = 0; i < data.Count; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                var dic = data[i];

                for (int j = 0; j < fields.Count; j++)
                {
                    var fieldName = fields[j].Name;
                    row.CreateCell(j).SetCellValue(dic[fieldName]?.ToString() ?? "");
                }
            }

            using var ms = new MemoryStream();
            wb.Write(ms);
            return ms.ToArray();
        }

        #endregion

        #region 获取服务方法返回的字段，用于前端设置导出栏位
        /// <summary>
        /// 根据服务类名+方法名，自动获取方法返回DTO的所有字段
        /// </summary>
        public List<string> GetReturnFieldsByMethod(string serviceFullName, string methodName)
        {
            // 1. 查找服务类型
            Type? serviceType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                serviceType = assembly.GetType(serviceFullName);
                if (serviceType != null) break;
            }
            if (serviceType == null)
                throw new UserFriendlyException("找不到服务：" + serviceFullName);

            // 2. 查找方法
            var method = serviceType.GetMethod(methodName);
            if (method == null)
                throw new UserFriendlyException("找不到方法：" + methodName);

            // 3. 解析真实返回的DTO类型（自动识别 Task / PageResult / List）
            Type realDtoType = GetRealDtoType(method.ReturnType);

            // 4. 返回所有属性名
            return realDtoType.GetProperties().Select(p => p.Name).ToList();
        }

        /// <summary>
        /// 自动解析真实返回的DTO类型
        /// </summary>
        private Type GetRealDtoType(Type returnType)
        {
            // 处理 Task<T>
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            // 处理 PagedResultDto<T>
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(PagedResultDto<>))
            {
                return returnType.GetGenericArguments()[0];
            }

            // 处理 List<T> / IEnumerable<T>
            if (typeof(IEnumerable).IsAssignableFrom(returnType) && returnType.IsGenericType)
            {
                return returnType.GetGenericArguments()[0];
            }
            return returnType;
        }

        #endregion
    }
}
