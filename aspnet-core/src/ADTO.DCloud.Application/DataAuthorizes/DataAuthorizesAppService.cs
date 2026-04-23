
using ADTO.DCloud.Authorization;
using ADTO.DCloud.DataAuthorizes.Dto;
using ADTO.DCloud.DataAuthorizes.Model;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes
{
    /// <summary>
    /// 数据权限
    /// </summary>
    [ADTOSharpAuthorize]
    public class DataAuthorizesAppService : DCloudAppServiceBase, IDataAuthorizesAppService
    {
        private readonly IRepository<DataAuthorize, Guid> _repository;
        private readonly IRepository<OrganizationUnit, Guid> _organizationRepository;
        private readonly DataFilterService _dataFilterService;

        public DataAuthorizesAppService(IRepository<DataAuthorize, Guid> repository,
            IRepository<OrganizationUnit, Guid> organizationRepository,
            DataFilterService dataFilterService)
        {
            _repository = repository;
            _organizationRepository = organizationRepository;
            _dataFilterService = dataFilterService;
        }
        #region 获取数据
        /// <summary>
        /// 获取分页列表数据权限规则数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetDataAuthorizeGetAllPageList")]
        public async Task<PagedResultDto<DataAuthorizeDto>> GetAllPageListAsync(GeDataAuthorizePagedInput input)
        {
            var query = _repository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Code.Equals(input.KeyWord));
            // 获取当前方法的特性
            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetAllPageListAsync)));
            query = await _dataFilterService.CreateDataFilteredQuery(query, permissionCode);

            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<DataAuthorizeDto>>(list);
            return new PagedResultDto<DataAuthorizeDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 根据Id获取数据权限数据(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataAuthorizeDto> GetAsync(EntityDto<Guid> input)
        {
            var authorize = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<DataAuthorizeDto>(authorize);
        }
        /// <summary>
        /// 根据Code获取数据权限数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataAuthorizeDto> GetByCodeAsync(EntityDto<string> input)
        {
            var authorize = await _repository.GetAll().Where(t => t.Code == input.Id).ToListAsync();
            return ObjectMapper.Map<DataAuthorizeDto>(authorize);
        }
        /// <summary>
        /// 根据角色Id获取数据授权数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<DataAuthorize>> GetList(string code, Guid[] objectIds)
        {
            var query = _repository.GetAll().Where(d => d.Code == code && objectIds.Contains(d.ObjectId));
            return await query.ToListAsync();
        }
        /// <summary>
        /// 获取当前用户的所有角色数据权限
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<List<DataAuthorizeDto>> GetDataAuthorizeByName(string code)
        {
            if (ADTOSharpSession.GetUserId() == Guid.Empty)
            {
                return new List<DataAuthorizeDto>();
            }
            var user = await UserManager.GetUserByIdAsync(ADTOSharpSession.GetUserId());
            var roles = await UserManager.GetUserRolesAsync(user);
            var objectIds = roles.Select(r => r.Id);

            var query = _repository.GetAll().Where(q => q.Code.Equals(code) && ((q.ObjectType == 1 && objectIds.Contains(q.ObjectId)) || (q.ObjectType == 2 && q.ObjectId == user.Id))).ToList();

            var ResultList = ObjectMapper.Map<List<DataAuthorizeDto>>(query);

            return ResultList;
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 新增数据权限规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataAuthorizeDto> CreateAsync(CreateDataAuthorizeDto input)
        {
            var dto = ObjectMapper.Map<DataAuthorize>(input);
            await _repository.InsertAsync(dto);
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<DataAuthorizeDto>(dto);
        }
        /// <summary>
        /// 修改数据权限规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataAuthorizeDto> UpdateAsync(UpdateDataAuthorizeDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        /// <summary>
        /// 删除数据权限规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpDelete("workflow/scheme/{id}")]
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);

        }
        #endregion

        #region 获取枚举数据
        /// <summary>
        /// 获取数据权限比较符
        /// </summary>
        /// <returns></returns>
        public List<EnumHelper.EnumDto> GetEnumConditionOperatorAsync()
        {
            var list = EnumHelper.GetEnumList<EnumConditionOperator>();

            return list;
        }
        /// <summary>
        /// 获取登录者信息字段枚举（用于动态查询或权限控制）
        /// </summary>
        /// <returns></returns>
        public List<EnumHelper.EnumDto> GetEnumLoginUserFieldAsync()
        {
            var list = EnumHelper.GetEnumList<EnumLoginUserField>();
            return list;
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        private string GetRequestPath()
        {
            return IocManager.Instance.Resolve<IHttpContextAccessor>().HttpContext.Request.Path;
        }

        /// <summary>
        /// 创建数据查询,返回带字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task<IQueryable<T>> CreateDataFilteredQuery<T>(IQueryable<T> query, string methodName)
        {
            //获取用户管理员,如果是系统默认管理员不限制数据权限，查所有
            var user = await UserManager.GetAdminAsync();
            if (user.Id == ADTOSharpSession.GetUserId())
                return query;
            var dataAuth = await this.GetDataAuthorizeByName(methodName);

            if (dataAuth.Count() < 1)
                return query;

            var model = new ParseConditionModel();
            Expression expression = null;
            var parameter = Expression.Parameter(typeof(T), "x");
            var expressions = new Dictionary<string, Expression>();
            int index = 1;
            try
            {
                foreach (var auth in dataAuth)
                {
                    var formulaModel = System.Text.Json.JsonSerializer.Deserialize<FormulaModel>(auth.Formula);
                    if (formulaModel.conditions == null)
                        break;
                    //如果条件公式不为空
                    if (string.IsNullOrEmpty(formulaModel.formula))
                    {
                        // 默认公式
                        for (int i = 1; i < formulaModel.conditions.Count + 1; i++)
                        {
                            var conditionItem = formulaModel.conditions[i - 1];
                            conditionItem.index = i;

                            if (!string.IsNullOrWhiteSpace(model.dynamicExpression))
                            {
                                model.dynamicExpression += " AND ";
                            }
                            model.dynamicExpression += $" adto{index}_{i} ";
                        }
                    }
                    else
                    {
                        // 默认公式
                        for (int i = 1; i < formulaModel.conditions.Count + 1; i++)
                        {
                            formulaModel.formula = formulaModel.formula.Replace($"{i}", $"adto{index}_{i}");
                        }
                        if (string.IsNullOrEmpty(model.dynamicExpression))
                            model.dynamicExpression += $" ({formulaModel.formula})";
                        else
                            model.dynamicExpression += $" AND ({formulaModel.formula})";
                    }
                    // 为每个条件创建表达式
                    await BuildPredicate(formulaModel, model, index, parameter, expressions);
                    index++;
                }
                ;
                var formula = model.dynamicExpression.Replace(" ", "");
                var finalExpression = ParseFormula(formula, expressions);
                var filteredQuery = query.Where(Expression.Lambda<Func<T, bool>>(finalExpression, parameter));
                return filteredQuery;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("数据权限报错:" + ex.Message);
            }
        }
        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <param name="config"></param>
        /// <param name="model"></param>
        /// <param name="param"></param>
        /// <param name="sqlNum"></param>
        /// <param name="parameter"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Dictionary<string, Expression>> BuildPredicate(FormulaModel config, ParseConditionModel model, int authNum, ParameterExpression parameter, Dictionary<string, Expression> expressions)
        {
            // 为每个条件创建表达式
            int index = 1;
            Expression conditionExpression = null;
            foreach (var conditionItem in config.conditions)
            {
                Expression property = parameter;
                foreach (var part in conditionItem.FieldId.Split('.'))
                {
                    var propInfo = property.Type.GetProperty(part);
                    if (propInfo == null)
                        throw new ArgumentException($"Property {part} not found on type {property.Type.Name}");
                    property = Expression.Property(property, propInfo);
                }
                var propertyType = property.Type;
                if (Nullable.GetUnderlyingType(propertyType) != null)
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }
                var paramValue = await this.GetFieldValues(conditionItem.FiledValueType, conditionItem.FiledValue);
                var constant = Expression.Constant(ConvertValue(paramValue, propertyType));
                switch (conditionItem.Symbol)
                {
                    case (int)EnumConditionOperator.Equal: //等于
                        Expression propertyExpr = property; // 假设 property 是 Expression
                        if (Nullable.GetUnderlyingType(property.Type) != null && property.Type != propertyType)
                        {
                            propertyExpr = Expression.Property(property, "Value"); // 提取 Nullable<T>.Value
                            conditionExpression = Expression.Equal(propertyExpr, constant);  // 比较 Guid == Guid
                        }
                        else
                            conditionExpression = Expression.Equal(property, constant);
                        break;
                    case (int)EnumConditionOperator.NotEqual://不等于
                        if (Nullable.GetUnderlyingType(property.Type) != null && property.Type != propertyType)
                        {
                            propertyExpr = Expression.Property(property, "Value"); // 提取 Nullable<T>.Value
                            conditionExpression = Expression.Equal(propertyExpr, constant);  // 比较 Guid == Guid
                        }
                        else
                            conditionExpression = Expression.NotEqual(property, constant);
                        break;
                    case (int)EnumConditionOperator.GreaterThan://大于

                        conditionExpression = Expression.NotEqual(property, constant);
                        break;
                    case (int)EnumConditionOperator.GreaterThanOrEqual://大于等于
                        conditionExpression = Expression.GreaterThanOrEqual(property, constant);
                        break;
                    case (int)EnumConditionOperator.LessThan://小于
                        conditionExpression = Expression.LessThan(property, constant);
                        break;
                    case (int)EnumConditionOperator.LessThanOrEqual://小于等于
                        conditionExpression = Expression.LessThanOrEqual(property, constant);
                        break;
                    case (int)EnumConditionOperator.Contains://包含
                        conditionExpression = BuildContainsExpression(property, constant);
                        break;
                    case (int)EnumConditionOperator.NotContains://不包含
                        conditionExpression = Expression.Not(BuildContainsExpression(property, constant));
                        break;
                    case (int)EnumConditionOperator.In://包含于
                        conditionExpression = BuildInExpression(property, conditionItem, paramValue);
                        break;
                    case (int)EnumConditionOperator.NotIn://不包含于
                        conditionExpression = BuildInExpression(property, conditionItem, paramValue);
                        break;
                }
                expressions.Add($"adto{authNum}_{index}", conditionExpression);
                index++;
            }
            return expressions;
        }
        private object ConvertValue(object value, Type targetType)
        {
            // 处理DBNull和null值（可空类型返回null，非可空类型抛出异常）
            if (value == null || value is DBNull)
            {
                return targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? null
                    : throw new InvalidOperationException($"Cannot convert null to non-nullable type {targetType.Name}");
            }
            // 如果已经是目标类型，直接返回
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }
            try
            {
                // 特殊处理字符串（避免调用Parse方法）
                if (targetType == typeof(string))
                {
                    return value.ToString();
                }
                // 处理可空类型（提取基础类型）
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                // 根据类型转换
                if (underlyingType == typeof(int))
                {
                    return int.Parse(value.ToString());
                }
                else if (underlyingType == typeof(Guid))
                {
                    return Guid.Parse(value.ToString());
                }
                else if (underlyingType == typeof(DateTime))
                {
                    return DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
                }
                else if (underlyingType == typeof(bool))
                {
                    return bool.Parse(value.ToString());
                }
                else if (underlyingType == typeof(decimal))
                {
                    return decimal.Parse(value.ToString(), CultureInfo.InvariantCulture);
                }
                else if (underlyingType.IsEnum)
                {
                    return Enum.Parse(underlyingType, value.ToString());
                }
                // 尝试使用Convert.ChangeType作为后备方案
                return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    $"Failed to convert value '{value}' ({value.GetType().Name}) to type {targetType.Name}",
                    ex);
            }
        }

        private Expression BuildContainsExpression(Expression property, ConstantExpression constant)
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            return Expression.Call(property, containsMethod, constant);
        }
        /// <summary>
        /// 保护于和不包含于处理
        /// </summary>
        /// <param name="property"></param>
        /// <param name="condition"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static Expression BuildInExpression(Expression property, ConditionModel condition, object values)
        {
            // 1. 处理空值情况
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            // 2. 获取值数组并确保非空
            var valueArray = (values as IEnumerable)?.Cast<object>().ToArray();
            if (valueArray == null || valueArray.Length == 0)
                return Expression.Constant(false); // 空列表直接返回false

            // 3. 获取属性类型（优先使用property的类型）
            var propertyType = property.Type;
            if (Nullable.GetUnderlyingType(propertyType) != null)
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }
            // 4. 转换值为property的类型
            var listType = typeof(List<>).MakeGenericType(propertyType);
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");

            foreach (var value in valueArray)
            {
                object convertedValue;
                try
                {
                    // 处理DBNull和null
                    if (value == null || value is DBNull)
                    {
                        convertedValue = null;
                    }
                    else
                    {
                        // 尝试类型转换
                        convertedValue = Convert.ChangeType(value, propertyType);
                    }
                    addMethod.Invoke(list, new[] { convertedValue });
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to convert value '{value}' to type {propertyType.Name}", ex);
                }
            }
            // 5. 获取Contains方法
            var containsMethod = listType.GetMethod("Contains", new[] { propertyType });

            // 6. 构建表达式
            var containsCall = Expression.Call(
                Expression.Constant(list),
                containsMethod,
                property.Type != propertyType
                    ? Expression.Convert(property, propertyType) // 处理Nullable类型
                    : property);

            // 7. 处理NOT IN情况
            return condition.Symbol == (int)EnumConditionOperator.In
                ? containsCall
                : Expression.Not(containsCall);

            #region MyRegion

            //// 转换值为正确的类型
            //var convertedValues = values.Select(v => v).ToArray();
            //var valueType = convertedValues.FirstOrDefault()?.GetType() ?? typeof(string);
            //// 创建包含这些值的常量表达式
            //var listType = typeof(List<>).MakeGenericType(valueType);
            //var list = Activator.CreateInstance(listType);
            //var addMethod = listType.GetMethod("Add");
            //foreach (var value in convertedValues)
            //{
            //    addMethod.Invoke(list, new[] { value });
            //}
            //var containsMethod = listType.GetMethod("Contains", new[] { valueType });
            //// 构建Contains调用表达式
            //var containsCall = Expression.Call(Expression.Constant(list), containsMethod, property);
            //// 对于NOT IN，取反
            //return condition.Symbol == (int)EnumConditionOperator.In
            //    ? containsCall
            //    : Expression.Not(containsCall);
            #endregion
        }
        /// <summary>
        /// 公式解析
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private Expression ParseFormula(string formula, Dictionary<string, Expression> expressions)
        {
            // 去除所有空格（简化处理）
            formula = formula.Replace(" ", "");

            // 第一步：处理括号
            int bracketStart = formula.LastIndexOf('(');
            while (bracketStart != -1)
            {
                int bracketEnd = formula.IndexOf(')', bracketStart);
                if (bracketEnd == -1) throw new ArgumentException("括号不匹配");

                // 提取括号内的子表达式
                string innerFormula = formula.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);

                // 递归解析子表达式
                Expression innerExpression = ParseFormula(innerFormula, expressions);

                // 为子表达式生成临时键（使用Guid避免冲突）
                string tempKey = "_" + Guid.NewGuid().ToString("N");
                expressions.Add(tempKey, innerExpression);

                // 替换原括号内容为临时键
                formula = formula.Substring(0, bracketStart) + tempKey + formula.Substring(bracketEnd + 1);

                // 继续查找剩余括号
                bracketStart = formula.LastIndexOf('(');
            }

            // 第二步：按优先级处理运算符
            int orIndex = formula.IndexOf("or", StringComparison.OrdinalIgnoreCase);
            while (orIndex != -1)
            {
                string left = formula.Substring(0, orIndex);
                string right = formula.Substring(orIndex + 2);

                return Expression.OrElse(
                    ParseFormula(left, expressions),
                    ParseFormula(right, expressions)
                );
            }

            int andIndex = formula.IndexOf("and", StringComparison.OrdinalIgnoreCase);
            while (andIndex != -1)
            {
                string left = formula.Substring(0, andIndex);
                string right = formula.Substring(andIndex + 3);

                return Expression.AndAlso(
                    ParseFormula(left, expressions),
                    ParseFormula(right, expressions)
                );
            }

            // 第三步：处理最终条件（字符串键）
            if (expressions.ContainsKey(formula))
            {
                return expressions[formula];
            }

            throw new ArgumentException($"无效的条件键: {formula}");
        }
        /// <summary>
        /// 按指定类型获取值域
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<object> GetFieldValues(int? type, string value)
        {
            var currentUser = await UserManager.GetUserByIdAsync(ADTOSharpSession.GetUserId());
            switch (type)
            {
                case (int)EnumLoginUserField.Text://文本
                    return value;
                case (int)EnumLoginUserField.UserId://登录者Id
                    return currentUser.Id.ToString();
                case (int)EnumLoginUserField.UserName://登录者账号
                    return currentUser.UserName;
                case (int)EnumLoginUserField.Company://登录公司
                    return currentUser.CompanyId.ToString();
                case (int)EnumLoginUserField.CompanyIn://登录公司及下属公司
                    var companyList = ObjectMapper.Map<List<OrganizationUnitDto>>(await _organizationRepository.GetAll().Where(w => w.Id == currentUser.CompanyId || w.ParentId == currentUser.CompanyId).ToListAsync());
                    var companyData = GetLoginUserAllOrganizations(companyList, null);
                    var companyResult = companyData.Select(s => s.Id).ToList();
                    companyResult.Insert(0, currentUser.CompanyId.Value);
                    return companyResult.Distinct().ToList();
                case (int)EnumLoginUserField.CompanyFixed://固定公司
                    return value;
                case (int)EnumLoginUserField.Department://登录者部门
                    return currentUser.DepartmentId.ToString();
                case (int)EnumLoginUserField.DepartmentIn://登录者部门及下属部门
                    var departmentList = ObjectMapper.Map<List<OrganizationUnitDto>>(await _organizationRepository.GetAll().Where(w => w.Id == currentUser.DepartmentId || w.ParentId == currentUser.DepartmentId).ToListAsync());
                    var departmentData = GetLoginUserAllOrganizations(departmentList, null);
                    var departResult = departmentData.Select(s => s.Id).ToList();
                    departResult.Insert(0, currentUser.DepartmentId.Value);
                    return departResult.Distinct().ToList();
                case (int)EnumLoginUserField.DepartmentFixed://固定部门
                    return value;
                case (int)EnumLoginUserField.PostId://登录者岗位
                    var postId = await UserManager.GetPostId();
                    return postId.ToString();
                case (int)EnumLoginUserField.Role://登录者角色
                    var user = await UserManager.GetUserByIdAsync(ADTOSharpSession.GetUserId());
                    var userRoleList = await UserManager.GetUserRolesAsync(user);
                    return userRoleList.Select(t => t.Id).Distinct().ToList();
            }
            return value;
        }

        /// <summary>
        /// 组织架构及下属组织架构
        /// </summary>
        /// <returns></returns>
        private List<OrganizationUnitDto> GetLoginUserAllOrganizations(List<OrganizationUnitDto> list, Guid? id)
        {
            var result = new List<OrganizationUnitDto>();
            var query = from c in list select c;
            if (id.HasValue)
                query = query.Where(w => w.ParentId == id);
            result.AddRange(query.ToList());
            foreach (var item in query.ToList())
            {
                result.AddRange(GetLoginUserAllOrganizations(list, item.Id));
            }
            return result;
        }
        #endregion

        #region 获取服务方法返回Dto
        /// <summary>
        /// 获取方法返回dto字段
        /// </summary>
        /// <returns></returns>
        public async Task<List<PropertiesDto>>  GetDtoProperties(GetDtoPropertiesInput input)
        {
            // 获取属性及备注
            Type serviceType = Type.GetType(input.ServiceName);
            if (serviceType == null)
            {
                serviceType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(input.ServiceName))
                    .FirstOrDefault(t => t != null);
            }
            if (serviceType == null)
                throw new ADTOSharpException($"无法找到类型{input.ServiceName}");

            var props = DtoMetadataHelper.GetDtoPropertiesWithRemarks(serviceType, input.MethodName);
       
            return props;
        }
        #endregion

    }
}

