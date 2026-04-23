using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceLocations.Dto
{
    public class UpdateStateDto : EntityDto<Guid>
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
    }
}
