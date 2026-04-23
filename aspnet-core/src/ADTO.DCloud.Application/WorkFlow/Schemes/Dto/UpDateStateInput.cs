using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 禁用启用流程表单
    /// </summary>
    public class UpDateStateInput : EntityDto<Guid>
    {
        public bool State { get; set; }
    }
}
