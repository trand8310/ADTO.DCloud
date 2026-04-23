using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{

    public class UpdateSchemeInput
    {
        /// <summary>
        /// 流程信息
        /// </summary>
        public UpdateWorkFlowSchemeinfoInput Schemeinfo { get; set; }
        /// <summary>
        /// 流程模板信息
        /// </summary>
        public WorkFlowSchemeInput Scheme { get; set; }
        /// <summary>
        /// 流程模板授权信息
        /// </summary>
        public IEnumerable<CreateWorFlowSchemeAuthInput> SchemeAuthList { get; set; }
    }
}
