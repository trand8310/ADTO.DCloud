using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    public class UpdateProcessInput:EntityDto<Guid>
    {
        /// <summary>
        /// 流程模板
        /// </summary>
        public string Schema { get; set; }
    }
}
