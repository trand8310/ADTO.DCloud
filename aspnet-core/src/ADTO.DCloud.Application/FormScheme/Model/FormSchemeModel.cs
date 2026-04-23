using ADTO.DCloud.FormScheme.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Model
{
    /// <summary>
    /// 表单模板
    /// </summary>
    public class FormSchemeModel
    {
        /// <summary>
        /// 数据库编码
        /// </summary>
        public string DbCode { get; set; }
        /// <summary>
        /// 数据表单信息
        /// </summary>
        public List<FormDbTableInfo> Db { get; set; }
        /// <summary>
        /// 表单信息
        /// </summary>
        public FormInfoModel FormInfo { get; set; }
        /// <summary>
        /// 1 视图表单 0常规表单 2自动建表
        /// </summary>
        public int FormType { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }


        /// <summary>
        /// 自定义服务方法-通过对应的服务方法来保存表单信息
        /// </summary>
        public string MethodName { get; set; }
    }
}
