using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CodeRule.Dto
{
    /// <summary>
    /// 返回业务编码
    /// </summary>
    public class GenerateCodeOutput
    {
        /// <summary>
        /// 生成的编码
        /// </summary>
        public string GeneratedCode { get; set; }

        /// <summary>
        /// 规则编码
        /// </summary>
        public string RuleCode { get; set; } // 新增：返回规则编号
       
    }
}
