using ADTO.DCloud.WorkFlow.Delegate;
using ADTO.DCloud.WorkFlow.Tasks;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegates.Dto
{
    /// <summary>
    /// 新增委托
    /// </summary>
    public class CreateWrokFlowDelegateInput
    {
        /// <summary>
        /// 模板信息
        /// </summary>
        public List<string> SchemeInfoList { get; set; }

        /// <summary>
        /// 委托信息
        /// </summary>
        public CreateWrokFlowDelegateDto DelegateRule { get; set; }
    }
}
