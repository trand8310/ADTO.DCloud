using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Dto
{
    /// <summary>
    /// 数据dto
    /// </summary>
    public class PropertiesDto
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field{ get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 查看
        /// </summary>
        public bool View { get; set; } = true;
        /// <summary>
        /// 编辑
        /// </summary>
        public bool Update { get; set; } = true;
        /// <summary>
        /// 导出
        /// </summary>
        public bool Export { get; set; } = true;
    }
}
