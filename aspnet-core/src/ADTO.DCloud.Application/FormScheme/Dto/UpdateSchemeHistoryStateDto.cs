using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 更新模板
    /// </summary>
    public class UpdateSchemeHistoryStateDto
    {
        /// <summary>
        /// 模板信息主键
        /// </summary>
        public Guid SchemeInfoId { get; set; }

        /// <summary>
        /// 当前模板主键
        /// </summary>
        public Guid SchemeId { get; set; }

    }
}
