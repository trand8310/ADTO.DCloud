using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataBase.Model
{
    public class GetExistFiledInput
    {
        /// <summary>
        /// 主键值
        /// </summary>
        public string KeyValue { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 主键名
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 数据字段
        /// </summary>
        public string FiledsJson { get; set; }
        /// <summary>
        /// 不需要多租户
        /// </summary>
        public int? IsNotSass { get; set; }
    }
}
