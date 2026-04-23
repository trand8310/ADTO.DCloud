using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class WFSchemeDto
    {
        /// <summary>
        /// 流程信息
        /// </summary>
        public WorkFlowSchemeinfoDto Schemeinfo { get; set; }
        /// <summary>
        /// 流程模板信息
        /// </summary>
        public WorkFlowSchemeDto Scheme { get; set; }
        /// <summary>
        /// 流程模板授权信息
        /// </summary>
        public IEnumerable<WorFlowSchemeAuthDto>  SchemeAuthList { get; set; }
    }
}
