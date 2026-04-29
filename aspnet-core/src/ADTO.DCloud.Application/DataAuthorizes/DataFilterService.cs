
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes.Dto;
using ADTO.DCloud.DataAuthorizes.Model;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Organizations;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe.Radar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.OSS.Model.ListPartsResult;

namespace ADTO.DCloud.DataAuthorizes
{
    [ADTOSharpAuthorize]
    public class DataFilterService : DCloudAppServiceBase, IDataFilterService
    {
        private readonly IRepository<DataAuthorize, Guid> _repository;
        private readonly IRepository<OrganizationUnit, Guid> _organizationRepository;
        private readonly OrganizationUnitAppService _organizationAppService;
        public DataFilterService(IRepository<DataAuthorize, Guid> repository,
            IRepository<OrganizationUnit, Guid> organizationRepository,
            OrganizationUnitAppService organizationAppService)
        {
            _repository = repository;
            _organizationRepository = organizationRepository;
            _organizationAppService = organizationAppService;
        }
        #region 扩展方法
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
        /// <summary>
        /// 创建数据查询,返回带字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task<DataFilteredJoined<T>> CreateDataAuthorizesFilteredQuery<T>(IQueryable<T> query, string methodName)
        {
            var result = new DataFilteredJoined<T>() { DataAuthFields = "*" };
            result.Query = query;

            //获取用户管理员,如果是系统默认管理员不限制数据权限，查所有
            var user = await UserManager.GetAdminAsync();
            if (user.Id == ADTOSharpSession.GetUserId())
                return result;
            var dataAuth = await this.GetDataAuthorizeByName(methodName);
            if (dataAuth.Count() < 1)
                return result;
            var model = new ParseConditionModel();
            Expression expression = null;
            var parameter = Expression.Parameter(typeof(T), "x");
            var expressions = new Dictionary<string, Expression>();
            int index = 1;
            List<PropertiesDto> filedList = new List<PropertiesDto>();
            try
            {
                foreach (var auth in dataAuth)
                {
                    #region 字段权限
                    if (string.IsNullOrEmpty(auth.Fields))
                    {
                        result.DataAuthFields = filedList.Count() > 0 ? MergeWithCondition(filedList, auth.Fields.ToList<PropertiesDto>()).ToJson() : auth.Fields;// auth.Fields.ToList<PropertiesDto>();
                    }
                    //auth.Fields
                    #endregion

                    var formulaModel = System.Text.Json.JsonSerializer.Deserialize<FormulaModel>(auth.Formula.Replace(" ", ""));
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
                            model.dynamicExpression += $" or ({formulaModel.formula})";
                    }
                    // 为每个条件创建表达式
                    await BuildPredicate(formulaModel, model, index, parameter, expressions);
                    index++;
                }
                ;
                var formula = model.dynamicExpression.Replace(" ", "");
                var finalExpression = ParseFormula(formula, expressions);
                var filteredQuery = query.Where(Expression.Lambda<Func<T, bool>>(finalExpression, parameter));
                result.Query = filteredQuery;
                return result;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("数据权限报错:" + ex.Message);
            }
        }
        /// <summary>
        /// 合并两个数组，以 field 为键。
        /// 规则：
        /// 1. 若第一个数组中某元素的 view == false，则用第二个数组中同 field 的元素覆盖（整个对象）。
        /// 2. 将第二个数组中存在于第一个数组中不存在的元素追加到结果中。
        /// </summary>
        public static List<PropertiesDto> MergeWithCondition(List<PropertiesDto> first, List<PropertiesDto> second)
        {
            // 用字典存储第一个数组，便于查找
            var dict = first.ToDictionary(x => x.Field);
            foreach (var secondItem in second)
            {
                if (dict.TryGetValue(secondItem.Field, out var firstItem))
                {
                    // 如果第一个数组中的该项 view == false，则用第二个数组的整个对象覆盖
                    if (firstItem.View == false)
                    {
                        dict[secondItem.Field] = secondItem;
                    }
                    // 否则保留第一个数组的原值（不做任何修改）
                }
                else
                {
                    // 第二个数组有而第一个数组没有的项，直接添加
                    dict[secondItem.Field] = secondItem;
                }
            }
            return dict.Values.ToList();
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
                    var formulaModel = System.Text.Json.JsonSerializer.Deserialize<FormulaModel>(auth.Formula.Replace(" ", ""));
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
                Type propertyType = property.Type;
                foreach (var part in conditionItem.FieldId.Split('.'))
                {
                    var propInfo = propertyType.GetProperty(part);
                    if (propInfo == null)
                        throw new ArgumentException($"Property {part} not found on type {propertyType.Name}");

                    if (conditionItem.FieldId.Split('.').Length <= 2)
                        property = Expression.Property(property, propInfo);
                    propertyType = propInfo.PropertyType;
                    // 如果是集合类型（非 string），获取元素类型
                    if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }
                }
                if (Nullable.GetUnderlyingType(propertyType) != null)
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }
                var paramValue = await this.GetFieldValues(conditionItem.FiledValueType, conditionItem.FiledValue);

                switch (conditionItem.Symbol)
                {
                    case (int)EnumConditionOperator.Equal: //等于
                        Expression propertyExpr = property; // 假设 property 是 Expression
                        if (Nullable.GetUnderlyingType(property.Type) != null && property.Type != propertyType)
                        {
                            propertyExpr = Expression.Property(property, "Value"); // 提取 Nullable<T>.Value
                            conditionExpression = Expression.Equal(propertyExpr, Expression.Constant(ConvertValue(paramValue, propertyType)));  // 比较 Guid == Guid
                        }
                        else
                            conditionExpression = Expression.Equal(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.NotEqual://不等于
                        if (Nullable.GetUnderlyingType(property.Type) != null && property.Type != propertyType)
                        {
                            propertyExpr = Expression.Property(property, "Value"); // 提取 Nullable<T>.Value
                            conditionExpression = Expression.Equal(propertyExpr, Expression.Constant(ConvertValue(paramValue, propertyType)));  // 比较 Guid == Guid
                        }
                        else
                            conditionExpression = Expression.NotEqual(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.GreaterThan://大于

                        conditionExpression = Expression.NotEqual(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.GreaterThanOrEqual://大于等于
                        conditionExpression = Expression.GreaterThanOrEqual(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.LessThan://小于
                        conditionExpression = Expression.LessThan(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.LessThanOrEqual://小于等于
                        conditionExpression = Expression.LessThanOrEqual(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.Contains://包含
                        conditionExpression = BuildContainsExpression(property, Expression.Constant(ConvertValue(paramValue, propertyType)));
                        break;
                    case (int)EnumConditionOperator.NotContains://不包含
                        conditionExpression = Expression.Not(BuildContainsExpression(property, Expression.Constant(ConvertValue(paramValue, propertyType))));
                        break;
                    case (int)EnumConditionOperator.In://包含于

                        //conditionExpression = BuildInExpression(property, paramValue, propertyType,  itemPropertyPath, isArray);
                        conditionExpression = BuildInExpression(property, conditionItem.FieldId, paramValue, propertyType, parameter);
                        break;
                    case (int)EnumConditionOperator.NotIn://不包含于
                        conditionExpression = Expression.Not(BuildInExpression(property, conditionItem.FieldId, paramValue, propertyType, parameter));
                        //conditionExpression = Expression.Not(BuildInExpression(property, paramValue, propertyType,  itemPropertyPath, isArray));
                        break;
                }
                expressions.Add($"adto{authNum}_{index}", conditionExpression);
                index++;
            }
            return expressions;
        }


        #region 构件表达式树
        /// <summary>
        /// 动态构建 Any 表达式（适用于 User.OrganizationUnits.OrganizationUnitId）
        /// </summary>
        /// <param name="rootType">根实体类型（如 User）</param>
        /// <param name="collectionPath">集合属性路径（如 "OrganizationUnits"）</param>
        /// <param name="itemPropertyPath">集合元素的属性路径（如 "OrganizationUnitId"）</param>
        /// <param name="values">要比较的值（object 类型，支持单个值或集合）</param>
        /// <returns>返回 Expression</returns>
        public Expression BuildAnyExpression<TEntity>(
            string collectionPath,          // "OrganizationUnits"
            string itemPropertyPath,        // "OrganizationUnitId"
            object values                   // 可以是单个值或 IEnumerable
        )
        {
            // 1. 创建根参数表达式（如 u => ...）
            ParameterExpression rootParam = Expression.Parameter(typeof(TEntity), "u");

            // 2. 获取集合属性表达式（u.OrganizationUnits）
            Expression collectionExpr = GetNestedPropertyExpression(
                rootParam,
                collectionPath,
                out Type collectionType
            );

            // 3. 检查是否是 IEnumerable<T>
            Type elementType = collectionType.GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments()[0])
                .FirstOrDefault()
                ?? throw new ArgumentException($"属性 {collectionPath} 必须实现 IEnumerable<T>");

            // 4. 获取集合元素的属性表达式（ou.OrganizationUnitId）
            ParameterExpression itemParam = Expression.Parameter(elementType, "ou");
            Expression itemPropertyExpr = GetNestedPropertyExpression(
                itemParam,
                itemPropertyPath,
                out Type propertyType
            );

            // 5. 将 values 转换为目标类型的集合
            IEnumerable valueList = (values is IEnumerable enumerable && !(values is string))
                ? enumerable.Cast<object>()
                : new[] { values };

            // 6. 构建 OR 组合条件（ou.OrganizationUnitId == value1 || ...）
            Expression anyCondition = BuildOrEqualExpressions(
                itemPropertyExpr,
                valueList,
                propertyType
            );

            // 7. 构建 Lambda 表达式（ou => ou.OrganizationUnitId == value1 || ...）
            LambdaExpression itemLambda = Expression.Lambda(anyCondition, itemParam);

            // 8. 调用 Enumerable.Any
            MethodInfo anyMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .MakeGenericMethod(elementType);

            Expression anyCall = Expression.Call(anyMethod, collectionExpr, itemLambda);

            // 9. 返回最终的 Lambda 表达式（u => u.OrganizationUnits.Any(...)）
            return anyCall;
        }

        /// <summary>
        /// 安全获取嵌套属性表达式
        /// </summary>
        private Expression GetNestedPropertyExpression(
            Expression root,
            string propertyPath,
            out Type propertyType
        )
        {
            Expression expr = root;
            Type currentType = root.Type;

            foreach (var propName in propertyPath.Split('.'))
            {
                PropertyInfo propInfo = currentType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null)
                    throw new ArgumentException($"属性 {propName} 在类型 {currentType.Name} 中不存在");

                expr = Expression.Property(expr, propInfo);
                currentType = propInfo.PropertyType;
            }

            propertyType = currentType;
            return expr;
        }

        /// <summary>
        /// 构建多个 Equal 条件的 OR 组合
        /// </summary>
        private Expression BuildOrEqualExpressions(
            Expression property,
            IEnumerable values,
            Type targetType
        )
        {
            Expression condition = null;
            foreach (var value in values)
            {
                // 值类型转换
                object convertedValue = ConvertValue(value, targetType);

                // 构建 x.Property == value
                Expression equalExpr = convertedValue == null
                    ? Expression.Equal(property, Expression.Constant(null, targetType))
                    : Expression.Equal(property, Expression.Constant(convertedValue, targetType));

                // 组合 OR 条件
                condition = condition == null
                    ? equalExpr
                    : Expression.OrElse(condition, equalExpr);
            }

            return condition ?? Expression.Constant(false); // 空集合返回 false
        }

        #endregion


        #region value数据类型转换
        /// <summary>
        /// value数据类型转换
        /// </summary>
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
            if (targetType.IsInstanceOfType(value) || value is IList || value.GetType().IsArray)
            {
                return value;
            }
            try
            {

                #region MyRegion
                // 处理可空类型（提取基础类型）
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                // 特殊处理字符串（避免调用Parse方法）
                if (targetType == typeof(string))
                {
                    return value.ToString();
                }
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
                else if (underlyingType == typeof(double))
                {
                    return double.Parse(value.ToString(), CultureInfo.InvariantCulture);
                }
                else if (underlyingType.IsEnum)
                {
                    return Enum.Parse(underlyingType, value.ToString());
                }


                // 处理可空类型
                //Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return Convert.ChangeType(value, underlyingType);

                // 尝试使用Convert.ChangeType作为后备方案
                //return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    $"Failed to convert value '{value}' ({value.GetType().Name}) to type {targetType.Name}",
                    ex);
            }
        }

        #endregion

        #region 包含条件
        private Expression BuildContainsExpression(Expression property, ConstantExpression constant)
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            return Expression.Call(property, containsMethod, constant);
        }
        #endregion

        #region 包含于
        protected Expression BuildInExpressionOld(Expression property, object values, Type propertyType)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valueArray = (values as IEnumerable)?.Cast<object>().ToArray();
            if (valueArray == null || valueArray.Length == 0)
                return Expression.Constant(false);

            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            // 转换为目标类型的列表（确保类型完全匹配）
            var listType = typeof(List<>).MakeGenericType(underlyingType);
            var list = (System.Collections.IList)Activator.CreateInstance(listType);
            foreach (var value in valueArray)
            {
                if (value == null || value is DBNull)
                {
                    list.Add(null); // 处理 null 或 DBNull
                }
                else if (value is IConvertible convertibleValue)
                {
                    try
                    {
                        list.Add(Convert.ChangeType(convertibleValue, underlyingType));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"无法将值 '{value}' 从 {value.GetType()} 转换为 {underlyingType}", ex);
                    }
                }
                else if (underlyingType.IsInstanceOfType(value))
                {
                    list.Add(value); // 类型已匹配，直接添加
                }
                else
                {
                    throw new InvalidOperationException(
                        $"类型 {value.GetType()} 不支持转换为 {underlyingType}");
                }
            }


            // 使用 List<T>.Contains（EF Core 可翻译）
            var containsMethod = listType.GetMethod("Contains", new[] { underlyingType });
            Expression propertyToUse = Nullable.GetUnderlyingType(property.Type) != null
                ? Expression.Convert(property, underlyingType) // 处理Nullable
                : property;

            return Expression.Call(
                Expression.Constant(list),
                containsMethod,
                propertyToUse);
        }
        // 构建动态条件表达式的方法
        public Expression BuildInExpression(Expression property, string propertyName, object values, Type propertyType, ParameterExpression parameter)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valueArray = (values as IEnumerable)?.Cast<object>().ToArray();
            if (valueArray == null || valueArray.Length == 0)
                return Expression.Constant(false);

            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            // 转换为目标类型的列表（确保类型完全匹配）
            var listType = typeof(List<>).MakeGenericType(underlyingType);
            var list = (System.Collections.IList)Activator.CreateInstance(listType);
            foreach (var value in valueArray)
            {
                if (value == null || value is DBNull)
                {
                    list.Add(null); // 处理 null 或 DBNull
                }
                else if (value is IConvertible convertibleValue)
                {
                    try
                    {
                        list.Add(Convert.ChangeType(convertibleValue, underlyingType));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"无法将值 '{value}' 从 {value.GetType()} 转换为 {underlyingType}", ex);
                    }
                }
                else if (underlyingType.IsInstanceOfType(value))
                {
                    list.Add(value); // 类型已匹配，直接添加
                }
                else
                {
                    throw new InvalidOperationException(
                        $"类型 {value.GetType()} 不支持转换为 {underlyingType}");
                }
            }
            #region MyRegion
            // 2. 获取属性名称的第一部分（例如 "UserOrganizationUnits"）
            var propertyNames = propertyName.Split('.');
            // 使用 List<T>.Contains（EF Core 可翻译）
            var containsMethod = listType.GetMethod("Contains", new[] { underlyingType });

            var propertyExpression = Expression.Property(parameter, propertyNames[0]);
            // 4. 通过反射逐层获取嵌套属性
            for (int i = 1; i < propertyNames.Length - 1; i++)
            {
                propertyExpression = Expression.Property(propertyExpression, propertyNames[i]);
            }
            var collectionType = propertyExpression.Type;
            var elementType = collectionType.IsGenericType ? collectionType.GetGenericArguments()[0] : null;
            if (elementType == null)
            {
                Expression propertyToUse = Nullable.GetUnderlyingType(property.Type) != null ? Expression.Convert(property, underlyingType) : property;
                return Expression.Call(
                    Expression.Constant(list),
                    containsMethod,
                    propertyToUse);
            }
            else
            {
                var idsConstant = Expression.Constant(list);
                // 4. 获取元素类型中 OrganizationUnitId 属性的信息
                var idProperty = elementType.GetProperty(propertyNames.Last());
                if (idProperty == null)
                    throw new InvalidOperationException($"The element type '{elementType.Name}' does not have an '{propertyNames.Last()}' property.");
                var anyMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType);
                // 6. 获取集合元素的属性并创建表达式
                var elementParameter = Expression.Parameter(elementType, propertyNames.Last());
                var propertyAccess = Expression.Property(elementParameter, idProperty);
                var containsCall = Expression.Call(idsConstant, containsMethod, propertyAccess);
                // 7. 创建 LINQ Any 方法的调用表达式
                var anyExpression = Expression.Call(null, anyMethod, propertyExpression,
                    Expression.Lambda(containsCall, elementParameter));
                // 3. 构建最终的 Lambda 表达式
                return anyExpression;
            }
            #endregion

        }
        // 辅助方法：构建 OR 组合条件
        private Expression BuildOrEqualExpressions(Expression property, IEnumerable<object> values, Type targetType)
        {
            Expression condition = null;
            foreach (var value in values)
            {
                object convertedValue = ConvertValue(value, targetType);
                Expression equalExpr = convertedValue == null
                    ? Expression.Equal(property, Expression.Constant(null, targetType))
                    : Expression.Equal(property, Expression.Constant(convertedValue, targetType));

                condition = condition == null ? equalExpr : Expression.OrElse(condition, equalExpr);
            }
            return condition ?? Expression.Constant(false);
        }
        #endregion

        #region 公式解析
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
        #endregion

        #region 按指定类型获取值域
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
                    var companyList = await _organizationRepository.GetAll().Where(w => w.Id == currentUser.CompanyId || w.ParentId == currentUser.CompanyId).ToListAsync();
                    var companyData = GetLoginUserAllOrganizations(companyList, null);
                    var companyResult = companyData.Select(s => s.Id).ToList();
                    if (currentUser.CompanyId != null)
                        companyResult.Insert(0, currentUser.CompanyId.Value);
                    return companyResult.Distinct().ToList();
                case (int)EnumLoginUserField.CompanyFixed://固定公司
                    return value;
                case (int)EnumLoginUserField.Department://登录者部门
                    return currentUser.DepartmentId.ToString();
                case (int)EnumLoginUserField.DepartmentIn://登录者部门及下属部门
                    //var departmentList = currentUser.OrganizationUnits;
                    List<Guid> departmentList = new List<Guid>();
                    if (currentUser.DepartmentId != null)
                        departmentList.Insert(0, currentUser.DepartmentId.Value);

                    //获取用户所有部门
                    var organizationUnits = await UserManager.GetOrganizationUnitsAsync(currentUser);
                    if (organizationUnits.Count() < 1)
                    {
                        departmentList.Add(Guid.Empty);
                        return departmentList;
                    }
                    var departments = await _organizationRepository.GetAll().Where(t => t.Classification == 4).ToListAsync();
                    foreach (var item in organizationUnits)
                    {
                        departmentList.Add(item.Id);
                        var departmentData = GetLoginUserAllOrganizations(departments, item.Id);
                        var departResult = departmentData.Select(s => s.Id).ToList();
                        departmentList = departmentList.Union(departResult).ToList();
                    }
                    //var departmentList = await _organizationRepository.GetAll().Where(w => w.Id == currentUser.DepartmentId || w.ParentId == currentUser.DepartmentId).ToListAsync();
                    //var departmentData = GetLoginUserAllOrganizations(departmentList, null);
                    //var departResult = departmentData.Select(s => s.Id).ToList();
                    //if (currentUser.DepartmentId != null)
                    //    departResult.Insert(departResult.Count() + 1, currentUser.DepartmentId.Value);
                    //return departResult.Distinct().ToList();
                    return departmentList;
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
        #endregion

        #region 组织架构及下属组织架构
        /// <summary>
        /// 组织架构及下属组织架构
        /// </summary>
        /// <returns></returns>
        private List<OrganizationUnit> GetLoginUserAllOrganizations(List<OrganizationUnit> list, Guid? id)
        {
            var result = new List<OrganizationUnit>();
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
        /// <summary>
        /// 组织架构及下属组织架构
        /// </summary>
        /// <returns></returns>
        private IReadOnlyList<DepartmentInfoDto> GetLoginUserDepartments(IReadOnlyList<DepartmentInfoDto> list, Guid? id)
        {
            var result = new List<DepartmentInfoDto>();
            var query = from c in list select c;
            if (id.HasValue)
                query = query.Where(w => w.ParentId == id);
            result.AddRange(query.ToList());
            foreach (var item in query.ToList())
            {
                result.AddRange(GetLoginUserDepartments(list, item.Id));
            }
            return result;
        }
        #endregion
        #endregion
    }
}

