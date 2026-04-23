using ADTO.DCloud.FormScheme.Model;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.UI;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 版本表单处理方法
    /// </summary>
    public class FormHelper
    {
        /// <summary>
        /// 获取保存sql语句
        /// </summary>
        /// <param name="formSchemeModel"></param>
        /// <param name="dataJson"></param>
        /// <param name="pkey"></param>
        /// <param name="pkeyValue"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        public static List<FormDbTable> GetSaveSql(FormSchemeModel formSchemeModel, JObject dataJson, string pkey, string pkeyValue, bool isUpdate)
        {
            List<FormDbTable> list = new List<FormDbTable>();
            // 获取主子表
            FormDbTableInfo mainTable = formSchemeModel.Db.Find(t => t.Type == "main");
            // 对表组件按表进行分类
            Dictionary<string, List<FormComponent>> tableMap = new Dictionary<string, List<FormComponent>>();
            Dictionary<string, FormComponent> girdTableMap = new Dictionary<string, FormComponent>();
            Dictionary<string, FormComponent> componentMap = new Dictionary<string, FormComponent>();
            Dictionary<string, FormLabelValueModel> tableValueMap = new Dictionary<string, FormLabelValueModel>();
            if (string.IsNullOrEmpty(pkey))
            {
                // 考虑到用数据视图做列表，做删除，编辑时无法获取到主键的情况
                pkey = formSchemeModel.PrimaryKey;
            }

            foreach (var component in formSchemeModel.FormInfo.Components)
            {
                var table = component.Config.Table;
                if (string.IsNullOrEmpty(table))
                {
                    continue;
                }
                if (!tableMap.ContainsKey(table))
                {
                    tableMap[table] = new List<FormComponent>();
                }

                if (component.Type == "gridtable")
                {
                    // 是多行子表
                    girdTableMap.Add(table, component);
                }
                else
                {
                    tableMap[table].Add(component);
                }
                if (component.Id == pkey)
                {
                    pkey = component.Config.Field;
                }
                componentMap.Add(component.Id, component);
            }
            if (!tableMap.ContainsKey(mainTable.Name))
            {
                throw new UserFriendlyException("#NotLog#主表没有设置组件！");
            }
            list.AddRange(GetChildrenSaveSql(formSchemeModel, mainTable, tableMap, dataJson,
                pkey, pkeyValue, isUpdate, girdTableMap,
                tableValueMap));


            // 处理重复字段的校验
            if (formSchemeModel.FormInfo.Form.VRepeatList != null)
            {
                string vRepeatValueId = "";
                foreach (var item in formSchemeModel.FormInfo.Form.VRepeatList)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        var componentPropList = item.Value.Split(",");
                        string valueId = "";
                        string sql = "";
                        string message = "【";
                        foreach (var prop in componentPropList)
                        {
                            if (componentMap.ContainsKey(prop))
                            {
                                var component = componentMap[prop];
                                var value = dataJson[prop].ToString();
                                if (!string.IsNullOrEmpty(value))
                                {
                                    if (valueId == "")
                                    {
                                        valueId = component.Config.Table.ToLower();
                                        sql = $"select * from {component.Config.Table.ToLower()} where 1=1  ";

                                        if (isUpdate)
                                        {
                                            string[] pkeys = tableValueMap[component.Config.Table].Key.Split(",");
                                            string[] pkeyValues = tableValueMap[component.Config.Table].Value.Split(",");
                                            int pindex = 0;
                                            foreach (var pkeyItem in pkeys)
                                            {
                                                sql += $" AND [{pkeyItem}] != '{pkeyValues[pindex]}' ";
                                                pindex++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message += ",";
                                    }
                                    valueId += "_" + dataJson[prop].ToString();
                                    sql += $" AND [{component.Config.Field}] = '{value}' ";
                                    message += component.Config.Label;
                                }
                                else
                                {
                                    valueId = "";
                                    break;
                                }
                            }
                            else
                            {
                                valueId = "";
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(valueId))
                        {
                            if (!string.IsNullOrEmpty(vRepeatValueId))
                            {
                                vRepeatValueId += ",";
                            }
                            else
                            {
                                list[0].RepeatSqL = new List<string>();
                                list[0].RepeatMessage = new List<string>();
                            }
                            vRepeatValueId += valueId;
                            message += "】";
                            if (componentPropList.Length > 1)
                            {
                                message += "组合";
                            }
                            message += "值重复！";

                            list[0].RepeatValueId = vRepeatValueId;
                            //list[0].RepeatSqL.Add(sql + " {designer_SASSID_NOTT} ");
                            list[0].RepeatSqL.Add(sql);
                            list[0].RepeatMessage.Add(message);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取保存子表sql语句
        /// </summary>
        /// <param name="formSchemeModel"></param>
        /// <param name="mainTable"></param>
        /// <param name="tableMap"></param>
        /// <param name="dataJson"></param>
        /// <param name="pkey"></param>
        /// <param name="pkeyValue"></param>
        /// <param name="isUpdate"></param>
        /// <param name="girdTableMap"></param>
        /// <param name="tableValueMap"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static List<FormDbTable> GetChildrenSaveSql(
            FormSchemeModel formSchemeModel,
            FormDbTableInfo mainTable,
            Dictionary<string, List<FormComponent>> tableMap,
            JObject dataJson,
            string pkey,
            string pkeyValue,
            bool isUpdate,
            Dictionary<string, FormComponent> girdTableMap,
            Dictionary<string, FormLabelValueModel> tableValueMap)
        {
            List<FormDbTable> list = new List<FormDbTable>();
            var childTables = formSchemeModel.Db.FindAll(t => t.RelationName == mainTable.Name);

            var mainTableComponents = tableMap[mainTable.Name];
            foreach (var childTable in childTables)
            {
                var pKey = pkey.Split(',');
                if (!((IList)pKey).Contains(childTable.RelationField))
                {
                    var rComponent = mainTableComponents.Find(t => t.Config.Field == childTable.RelationField);
                    if (rComponent == null)
                    {
                        var newRComponent = new FormComponent()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Type = "guid",
                            Config = new FormComponentConfig()
                            {
                                Table = childTable.RelationName,
                                Field = childTable.RelationField,
                            }
                        };
                        dataJson.Add(newRComponent.Id, Guid.NewGuid().ToString());
                        // 关联建没有设置建值
                        mainTableComponents.Add(newRComponent);
                    }
                    else if (rComponent.Type != "guid" && dataJson[rComponent.Id].IsEmpty())
                    {
                        throw new UserFriendlyException($"#NotLog#子表{childTable.Name}关联键没有值,绑定组件为{rComponent.Type}！");
                    }
                }
            }
            if (isUpdate)
            {
                list.Add(GetUpDateSql(mainTable, tableMap, dataJson, pkey, pkeyValue));
                tableValueMap.Add(mainTable.Name, new FormLabelValueModel()
                {
                    Key = pkey,
                    Value = pkeyValue
                });
            }
            else
            {
                list.Add(GetInsertSql(mainTable, tableMap, dataJson));
            }

            foreach (var childTable in childTables)
            {
                if (tableMap.ContainsKey(childTable.Name))
                {
                    var rComponent = tableMap[childTable.RelationName].Find(t => t.Config.Field == childTable.RelationField);
                    FormComponent newComponent = new FormComponent();
                    newComponent.Config = new FormComponentConfig();
                    if (rComponent != null)
                    {
                        newComponent.Id = rComponent.Id;
                        newComponent.Config.Field = childTable.Field;
                        newComponent.Type = "guid";
                        tableMap[childTable.Name].Add(newComponent);
                    }
                    else
                    {
                        throw new UserFriendlyException("#NotLog#子表找不到关联值");
                    }
                    if (girdTableMap.ContainsKey(childTable.Name))
                    {
                        var girdTableComponent = girdTableMap[childTable.Name];
                        // 处理排序字段
                        if (!string.IsNullOrEmpty(girdTableComponent.Config.OrderId) && girdTableComponent.Config.OrderCsType.ToUpper() == "INT")
                        {
                            if (tableMap[childTable.Name].FindIndex(t => t.Config.Field == girdTableComponent.Config.OrderId) == -1)
                            {
                                tableMap[childTable.Name].Add(new FormComponent()
                                {
                                    Config = new FormComponentConfig()
                                    {
                                        Field = girdTableComponent.Config.OrderId,
                                        CsType = girdTableComponent.Config.OrderCsType,
                                    },
                                    Type = "designer_index"
                                });
                            }
                        }
                        List<JObject> girdDataJson = dataJson[girdTableComponent.Id].ToString().ToObject<List<JObject>>();
                        int index = 0;
                        foreach (var girdData in girdDataJson)
                        {
                            if (newComponent != null)
                            {
                                girdData.Add(newComponent.Id, dataJson[newComponent.Id]);
                            }
                            var tableFlag = "";
                            if (!girdData["designer_table_flag"].IsEmpty())
                            {
                                tableFlag = girdData["designer_table_flag"].ToString();
                            }
                            var childPkeyComponent = tableMap[childTable.Name].Find(t => t.Config.Field.ToLower() == childTable.PKey.ToLower());
                            if (girdData[childPkeyComponent?.Id].IsEmpty() && tableFlag != "add")
                            {
                                throw new UserFriendlyException($"【${childTable.Name}】子表没有主键数据！");
                            }
                            if (tableFlag == "add")
                            {
                                // 新增
                                list.Add(GetInsertSql(childTable, tableMap, girdData, index));
                            }
                            else if (tableFlag == "delete")
                            {
                                // 删除
                                list.Add(GetDeleteSql(childTable, girdData[childPkeyComponent.Id].ToString()));
                            }
                            else
                            {
                                // 更新
                                list.Add(GetUpDateSql(childTable, tableMap, girdData, childTable.PKey, girdData[childPkeyComponent.Id].ToString(), index));
                            }
                            index++;
                        }
                    }
                    else
                    {
                        list.AddRange(GetChildrenSaveSql(formSchemeModel, childTable, tableMap, dataJson, childTable.Field, dataJson[rComponent.Id].ToString(), isUpdate, girdTableMap, tableValueMap));
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 获取新增sql语句
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="tableMap">组件集合</param>
        /// <param name="dataJson">表单数据</param>
        /// <param name="sindex">子表排序字段</param>
        /// <returns></returns>
        private static FormDbTable GetInsertSql(FormDbTableInfo table, Dictionary<string, List<FormComponent>> tableMap, JObject dataJson, int sindex = 0)
        {
            FormDbTable res = new FormDbTable();
            res.DbParameter = new List<SqlParameter>();
            res.TableName = table.Name;
            res.ExecuteType = ExecuteType.Insert;

            foreach (var component in tableMap[table.Name])
            {
                var field = component.Config.Field;
                var csType = component.Config.CsType;

                if (field==null||res.DbParameter.Find(t => t.ParameterName.ToUpper() == field.ToUpper()) != null)
                {
                    continue;
                }

                if (component.Type == "designer_index")
                {
                    res.DbParameter.Add(GetMyDbParameter(field, sindex.ToString(), csType));
                }
                else if (!string.IsNullOrEmpty(field) && !dataJson[component.Id].IsEmpty())
                {
                    res.DbParameter.Add(GetMyDbParameter(field, dataJson[component.Id].ToString(), csType));
                }
            }
            return res;
        }

        /// <summary>
        /// 获取更新sql语句
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="tableMap">组件集合</param>
        /// <param name="dataJson">表单数据</param>
        /// <param name="pkey">主键</param>
        /// <param name="pkeyValue">主键值</param>
        /// <param name="sindex">子表排序字段</param>
        /// <returns></returns>
        private static FormDbTable GetUpDateSql(FormDbTableInfo table, Dictionary<string, List<FormComponent>> tableMap, JObject dataJson, string pkey, string pkeyValue, int sindex = 0)
        {
            FormDbTable res = new FormDbTable();

            res.DbParameter = new List<SqlParameter>();
            res.TableName = table.Name;
            res.ExecuteType = ExecuteType.Update;
            res.Pkey = pkey;

            var formComponents = tableMap[table.Name];


            string[] pkeys = res.Pkey.Split(",");
            string[] pkeyValues = pkeyValue.Split(",");

            // 添加组件数据
            foreach (var component in formComponents)
            {
                var field = component.Config.Field;
                var csType = component.Config.CsType;
                if (!string.IsNullOrEmpty(field))
                {
                    var id = component.Id;
                    if (res.DbParameter.Find(t => t.ParameterName.ToUpper() == field.ToUpper()) != null)
                    {
                        // 参数添加过则不再添加
                        continue;
                    }

                    if (component.Type == "designer_index")
                    {
                        res.DbParameter.Add(GetMyDbParameter(field, sindex.ToString(), csType));
                    }
                    else if (!dataJson[id].IsEmpty())
                    {
                        res.DbParameter.Add(GetMyDbParameter(field, dataJson[id].ToString(), csType));
                    }
                    else
                    {
                        res.DbParameter.Add(new SqlParameter(field, null));
                    }
                }
            }


            // 添加主键数据
            int index = 0;
            foreach (var keyItem in pkeys)
            {
                var parameter = res.DbParameter.Find(t => t.ParameterName.ToUpper() == keyItem.ToUpper());
                if (parameter == null)
                {
                    res.DbParameter.Add(new SqlParameter(keyItem, pkeyValues[index].Trim()));
                }
                else if (parameter.Value == null)
                {
                    parameter.Value = pkeyValues[index].Trim();
                }
                else
                {
                    parameter.Value = parameter.Value.ToString().Trim();
                }

                index++;
            }

            return res;
        }

        /// <summary>
        /// 获取删除sql语句
        /// </summary>
        /// <param name="table"></param>
        /// <param name="pkeyValue"></param>
        /// <returns></returns>
        private static FormDbTable GetDeleteSql(FormDbTableInfo table, string pkeyValue)
        {
            FormDbTable res = new FormDbTable();
            res.DbParameter = new List<SqlParameter>();
            res.TableName = table.Name;
            res.ExecuteType = ExecuteType.Delete;

            res.DbParameter.Add(new SqlParameter(table.PKey, pkeyValue));
            return res;
        }

        private static void GetDeleteSql(List<FormDbTable> list, List<FormDbTableInfo> db, string pTableName)
        {
            var tables = db.FindAll(t => t.RelationName == pTableName);
            foreach (var table in tables)
            {
                var sql = new StringBuilder();
                sql.Append($" DELETE FROM {table.Name} WHERE [{table.Field}] = @keyValue " );
                list.Add(new FormDbTable
                {
                    Sql = sql.ToString(),
                    RelationField = table.RelationField,
                    RelationName = table.RelationName,
                    TableName = table.Name
                });
                GetDeleteSql(list, db, table.Name);
            }
        }

        /// <summary>
        /// 获取表单查询语句
        /// </summary>
        /// <param name="list"></param>
        /// <param name="db"></param>
        /// <param name="pTableName"></param>
        /// <param name="tableMap"></param>
        /// <param name="girdTableMap"></param>
        private static void GetQuery(List<FormDbTable> list, List<FormDbTableInfo> db, string pTableName, Dictionary<string, List<FormComponent>> tableMap, Dictionary<string, FormComponent> girdTableMap)
        {
            var tables = db.FindAll(t => t.RelationName == pTableName);
            if (tables.Count == 0)
            {
                list.Find(t => t.TableName == pTableName).IsLast = true;
            }
            foreach (var table in tables)
            {
                StringBuilder str = new StringBuilder();
                str.Append("SELECT ");
                var components = tableMap[table.Name];
                foreach (var column in components)
                {
                    str.Append($" [{column.Config.Field}],");
                }
                str.Append($"FROM {table.Name} where {table.Field} = @keyValue ");

                if (girdTableMap.ContainsKey(table.Name))
                {
                    var componentModel = girdTableMap[table.Name];
                    if (!string.IsNullOrEmpty(componentModel.Config.OrderId))
                    {
                        if (componentModel.Config.IsDESC)
                        {
                            str.Append($" order by [{componentModel.Config.OrderId}] DESC ");
                        }
                        else
                        {
                            str.Append($" order by [{componentModel.Config.OrderId}] ");
                        }
                    }
                }

                var rTable = list.Find(t => t.TableName == table.RelationName);
                if (rTable.Sql.IndexOf(table.RelationField + ",") == -1 && rTable.Sql.IndexOf(table.RelationField + " FROM") == -1)
                {
                    rTable.Sql = rTable.Sql.Replace(" FROM", $",[{table.RelationField}] FROM");
                }

                list.Add(new FormDbTable
                {
                    Sql = str.ToString().Replace(",FROM", " FROM"),
                    RelationField = table.RelationField,
                    RelationName = table.RelationName,
                    TableName = table.Name
                });



                GetQuery(list, db, table.Name, tableMap, girdTableMap);
            }


        }
        /// <summary>
        /// 获取表单查询方法
        /// </summary>
        /// <param name="formSchemeModel"></param>
        /// <param name="pkey"></param>
        /// <param name="pkeyValue">组件值</param>
        /// <returns></returns>
        public static List<FormDbTable> GetQuery(FormSchemeModel formSchemeModel, string pkey, string pkeyValue)
        {
            if (string.IsNullOrEmpty(pkey))
            {
                // 考虑到用数据视图做列表，做删除，编辑时无法获取到主键的情况
                pkey = formSchemeModel.PrimaryKey;
            }
            List<FormDbTable> list = new List<FormDbTable>();
            StringBuilder str = new StringBuilder();
            FormDbTableInfo mainTable = formSchemeModel.Db.Find(t => t.Type == "main");
            // 对表组件按表进行分类
            Dictionary<string, List<FormComponent>> tableMap = new Dictionary<string, List<FormComponent>>();
            Dictionary<string, FormComponent> girdTableMap = new Dictionary<string, FormComponent>();
            foreach (var component in formSchemeModel.FormInfo.Components)
            {
                var table = component.Config.Table;
                if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(component.Config.Field) && component.Type != "gridtable")
                {
                    continue;
                }
                if (!tableMap.ContainsKey(table))
                {
                    tableMap[table] = new List<FormComponent>();
                }

                if (component.Type == "gridtable")
                {
                    girdTableMap.Add(table, component);
                }
                else
                {
                    if (component.Id == pkey)
                    {
                        pkey = component.Config.Field;
                    }
                    tableMap[table].Add(component);
                }
            }
            str.Append("SELECT ");
            var mainComponents = tableMap[mainTable.Name];
            foreach (var component in mainComponents)
            {
                str.Append($" [{component.Config.Field}],");
            }
            str.Append($"FROM {mainTable.Name} t where 1=1 ");
            if (string.IsNullOrEmpty(pkeyValue))
            {
                str.Append($" AND [{pkey}] = @keyValue ");
            }
            else
            {
                string[] pkeys = pkey.Split(",");
                string[] pkeyValues = pkeyValue.Split(",");
                int pindex = 0;
                foreach (var pkeyItem in pkeys)
                {
                    str.Append($" AND [{pkeyItem}] = '{pkeyValues[pindex]}' ");
                    pindex++;
                }
            }
            str.Append(" {ADTO_SASSID} ");
            list.Add(new FormDbTable
            {
                Sql = str.ToString().Replace(",FROM", " FROM"),
                TableName = mainTable.Name
            });
            GetQuery(list, formSchemeModel.Db, mainTable.Name, tableMap, girdTableMap);
            return list;
        }
        /// <summary>
        /// 获取表单列表数据的数据的查询字段
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableMap"></param>
        /// <param name="pTableName"></param>
        /// <returns></returns>
        private static string GetQueryColumns(List<FormDbTableInfo> db, Dictionary<string, List<FormComponent>> tableMap, string pTableName)
        {
            StringBuilder str = new StringBuilder();
            var tables = db.FindAll(t => t.RelationName == pTableName && tableMap.ContainsKey(t.Name));
            foreach (var table in tables)
            {
                int tableIndex = db.FindIndex(t => t.Name == table.Name);

                var components = tableMap[table.Name];

                foreach (var component in components)
                {
                    if (!string.IsNullOrEmpty(component.Config.Field))
                    {
                        str.Append(string.Format("t{0}.[{1}] as {1}{0},", tableIndex, component.Config.Field));
                    }
                }

                str.Append(GetQueryColumns(db, tableMap, table.Name));
            }

            return str.ToString();
        }

        /// <summary>
        /// 获取查询关联表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableMap"></param>
        /// <param name="pTableName"></param>
        /// <param name="pTableIndex"></param>
        /// <returns></returns>
        private static string GetQueryLeftTable(List<FormDbTableInfo> db, Dictionary<string, List<FormComponent>> tableMap, string pTableName, int? pTableIndex = null)
        {
            StringBuilder str = new StringBuilder();
            var tables = db.FindAll(t => t.RelationName == pTableName && tableMap.ContainsKey(t.Name));
            foreach (var table in tables)
            {

                int tableIndex = db.FindIndex(t => t.Name == table.Name);
                if (pTableIndex == null)
                {
                    str.Append(string.Format(" LEFT JOIN {0} t{1} ON t{1}.{2} = t.{3} ", table.Name, tableIndex, table.Field, table.RelationField));
                }
                else
                {

                    str.Append(string.Format(" LEFT JOIN {0} t{1} ON t{1}.{2} = t{3}.{4} ", table.Name, tableIndex, table.Field, pTableIndex, table.RelationField));
                }

                str.Append(GetQueryLeftTable(db, tableMap, table.Name, tableIndex));
            }

            return str.ToString();
        }

        private static SqlParameter GetMyDbParameter(string parameterName, string value, string csType)
        {
            string _csType = csType;
            switch (csType)
            {
                case "int":
                    _csType = "System.Int32";
                    break;
                case "long":
                    _csType = "System.Int64";
                    break;
                case "short":
                    _csType = "System.Int16";
                    break;
                case "bool":
                    _csType = "System.Boolean";
                    break;
                case "float":
                    _csType = "System.Single";
                    break;
                case "text":
                    _csType = "";
                    break;
                default:
                    if (!string.IsNullOrEmpty(csType))
                    {
                        _csType = $"System.{DtoSortingHelper.FirstUpper(csType)}";
                    }
                    break;
            }
            if (_csType == "System.TimeSpan")
            {
                return new SqlParameter(parameterName, TimeSpan.Parse(value));
            }
            else if (_csType == "System.Guid")
            {
                return new SqlParameter(parameterName, Guid.Parse(value));
            }
            else
            {
                return new SqlParameter(parameterName, string.IsNullOrEmpty(_csType) ? value : Convert.ChangeType(value, Type.GetType(_csType)));
            }
        }

    }
}
