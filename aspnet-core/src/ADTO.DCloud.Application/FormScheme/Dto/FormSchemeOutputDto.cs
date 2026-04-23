using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 获取动态表单详情返回实体
    /// </summary>
    public class FormSchemeOutputDto
    {
        /// <summary>
        /// 模板基础信息
        /// </summary>
        public FormSchemeInfoDto Info { get; set; }
        /// <summary>
        /// 模板信息
        /// </summary>
        public FormSchemeDto Scheme { get; set; }
    }
}
