using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegates.Dto
{
    /// <summary>
    /// 修改
    /// </summary>
    public class UpdateWrokFlowDelegateInput
    {
        /// <summary>
        /// 模板信息
        /// </summary>
        public List<string> SchemeInfoList { get; set; }

        /// <summary>
        /// 委托信息
        /// </summary>
        public UpdateWrokFlowDelegateDto DelegateRule { get; set; }
    }
}
