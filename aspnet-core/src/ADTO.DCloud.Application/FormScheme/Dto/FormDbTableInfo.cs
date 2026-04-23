using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 表单数据表信息
    /// </summary>
    public class FormDbTableInfo
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// SqlServer下是Owner、PostgreSQL下是Schema、MySql下是数据库名
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表备注，SqlServer下是扩展属性 MS_Description
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// main 主表 chlid 子表
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 关联字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 关联表
        /// </summary>
        public string RelationName { get; set; }
        /// <summary>
        /// 关联表字段
        /// </summary>
        public string RelationField { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string PKey { get; set; }

        /// <summary>
        /// 视图表单sql语句
        /// </summary>
        public string Sql { get; set; }
    }
}
