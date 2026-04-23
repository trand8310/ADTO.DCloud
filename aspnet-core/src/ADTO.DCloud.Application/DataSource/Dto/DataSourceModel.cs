using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataSource.Dto
{
    public class DataSourceModel
    {
        /// <summary>
        /// 类型 table 表格,sql sql语句,code js脚本,connect 多个数据视图关联
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 输入参数
        /// </summary>
        public List<DataSourceParamModel> QueryFields { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public List<DataSourceOrderModel> Orders { get; set; }
        /// <summary>
        /// 查询语句
        /// </summary>
        public List<DataSourceSqlModel> Sqls { get; set; }

        /// <summary>
        /// 列的信息
        /// </summary>
        public List<DataSourceColumns> Columns { get; set; }
        /// <summary>
        /// 脚本数据源参数
        /// </summary>
        public List<DataSourceInputModel> DataSourceInputs { get; set; }
        /// <summary>
        /// 列信息（之前的版本 2.1.8 版本将会去掉）
        /// </summary>
        public List<string> Cols { get; set; }
    }
    /// <summary>
    /// 脚本数据源参数
    /// </summary>
    public class DataSourceInputModel
    {
        /// <summary>
        /// 数据源编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据源类型
        /// </summary>
        public string Type { get; set; }
    }


    /// <summary>
    /// 数据视图列信息
    /// </summary>
    public class DataSourceColumns
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Prop { get; set; }

        /// <summary>
        /// 别名（预留字段)
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string CsType { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// 输入参数模型
    /// </summary>
    public class DataSourceParamModel
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型 string 字符串; int 数字;DateTime 时间
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        ///  说明
        /// </summary>
        public string Des { get; set; }
    }
    /// <summary>
    /// 排序字段
    /// </summary>
    public class DataSourceOrderModel
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool IsDESC { get; set; }
    }

    /// <summary>
    /// 查询语句
    /// </summary>
    public class DataSourceSqlModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string DbTable { get; set; }
        /// <summary>
        /// 数据库编码
        /// </summary>
        public string DbCode { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 类型 1关联查询 2子查询
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 子查询参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 与父级的关联关系
        /// </summary>
        public List<DataSourceRelationModel> Relations { get; set; }

    }
    /// <summary>
    /// 语句关联关系
    /// </summary>
    public class DataSourceRelationModel
    {
        /// <summary>
        /// 关联字段
        /// </summary>
        public string MyField { get; set; }
        /// <summary>
        /// 关联父级字段
        /// </summary>
        public string OtherField { get; set; }
    }

    /// <summary>
    /// 数据源结果集
    /// </summary>
    public class DataSourceRes
    {
        /// <summary>
        /// 数据库编码
        /// </summary>
        public string DbCode { get; set; }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public DataTable Data { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, object> ParamsDic { get; set; }
    }
    public class GetDataTableByValuesInput
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 请求ids
        /// </summary>
        public string Ids { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string paramsJson { get; set; }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    public class DataQueryInputModel
    {
        /// <summary>
        /// 查询参数
        /// </summary>
        public string ParamsJson { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sidx { get; set; }
    }
}
