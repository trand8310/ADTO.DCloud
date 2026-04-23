using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapTo(typeof(WorkFlowScheme))]
    public class WorkFlowSchemeInput
    {
        /// <summary> 
        /// 1.正式2.草稿 
        /// </summary> 
        public int? Type { get; set; }
        /// <summary> 
        /// 流程模板内容 
        /// </summary> 
        [StringLength(int.MaxValue)]
        public string Content { get; set; }
    }
}
