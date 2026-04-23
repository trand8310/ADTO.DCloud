using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    public class UpdateWorkFLowLogRemarkInput:EntityDto<Guid>
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
