using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class FormDbTable
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// (更新,查询)条件字段名
        /// </summary>
        public string Pkey { get; set; }
        /// <summary>
        /// 执行操作类型
        /// </summary>
        public ExecuteType ExecuteType { get; set; }
        /// <summary>
        /// 执行参数
        /// </summary>
        public List<SqlParameter> DbParameter { get; set; }
        /// <summary>
        /// 查询语句
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 关联表
        /// </summary>
        public string RelationName { get; set; }
        /// <summary>
        /// 关联表字段
        /// </summary>
        public string RelationField { get; set; }
        /// <summary>
        /// 最后的查询关联项
        /// </summary>
        public bool IsLast { get; set; }


        #region 判断重复字段标记
        /// <summary>
        /// 需要判断重复字段的值
        /// </summary>
        public string RepeatValueId { get; set; }
        /// <summary>
        /// 判断是否有重复值的sql语句
        /// </summary>
        public List<string> RepeatSqL { get; set; }
        /// <summary>
        /// 判断重复值的消息提醒
        /// </summary>
        public List<string> RepeatMessage { get; set; }
        #endregion
    }
}
