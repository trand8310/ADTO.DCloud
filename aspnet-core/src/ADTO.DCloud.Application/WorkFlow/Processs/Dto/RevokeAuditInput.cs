using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    public class RevokeAuditInput 
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public Guid processId { get; set; }
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }
    }
}
