using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    public class UpdateSchemeDto:EntityDto<Guid>
    {
        /// <summary>
        /// 流程模板Id
        /// </summary>
        public  Guid SchemeId { get; set; }
    }
}
