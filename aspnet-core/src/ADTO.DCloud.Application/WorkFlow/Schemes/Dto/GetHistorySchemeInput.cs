using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 获取流程模板历史记录-不分页
    /// </summary>
    public class GetHistorySchemeInput
    {
        /// <summary>
        /// 流程模板信息主键
        /// </summary>
        public Guid SchemeInfoId { get; set; }
    }
}
