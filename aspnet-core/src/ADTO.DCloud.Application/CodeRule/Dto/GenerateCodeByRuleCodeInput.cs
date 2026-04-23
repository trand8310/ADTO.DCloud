using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CodeRule.Dto
{
    public class GenerateCodeByRuleCodeInput
    {
        /// <summary>
        /// 编码规则编号（如ORDER_CODE、CONTRACT_CODE，必填）
        /// </summary>
        [Required(ErrorMessage = "编码规则编号不能为空")]
        public string RuleCode { get; set; }

        /// <summary>
        /// 业务ID（如订单ID/合同ID，可选，用于防重）
        /// </summary>
        public Guid? BusinessId { get; set; }

        
    }
}
