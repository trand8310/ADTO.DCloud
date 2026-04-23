using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Dto
{
    /// <summary>
    /// 申请单禁用用户
    /// </summary>
    [AutoMap(typeof(DisableUserWrokFlow))]
    public class DisableUserWrokFlowDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 申请单类型
        /// </summary>
        public string ResourceTable { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
