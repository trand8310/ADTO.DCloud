using ADTO.DCloud.Attendances.AttendanceTimeRules.Dto;
using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceTimeRules
{
    /// <summary>
    /// 考勤时间
    /// </summary>
    public interface IAttendanceTimeRuleAppService : IApplicationService
    {

        /// <summary>
        /// 获取考勤时间数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<AttendanceTimeRuleDto>> GetAllAsync(GetAttendanceTimeRuleInput input);
    }
}
