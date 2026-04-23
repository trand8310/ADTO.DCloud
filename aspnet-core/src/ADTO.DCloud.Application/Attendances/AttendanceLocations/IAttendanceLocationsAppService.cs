using ADTO.DCloud.Attendances.AttendanceLocations.Dto;
using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceLocations
{
    public interface IAttendanceLocationsAppService : IApplicationService
    {
        /// <summary>
        /// 获取办公地点数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<AttendanceLocationDto>> GetAllAsync(GetAttendanceLocationInput input);
    }
}
