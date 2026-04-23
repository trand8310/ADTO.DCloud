using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Model
{
    /// <summary>
    /// 公司
    /// </summary>
    public class FormulaModel
    {
        /// <summary>
        /// 设置公式
        /// </summary>
        public string formula { get; set; }
        /// <summary>
        /// 设置条件
        /// </summary>
        public List<ConditionModel> conditions { get; set; }
        /// <summary>
        /// 分组公式
        /// </summary>
        public List<GroupModel> formulas { get; set; }
    }
}
