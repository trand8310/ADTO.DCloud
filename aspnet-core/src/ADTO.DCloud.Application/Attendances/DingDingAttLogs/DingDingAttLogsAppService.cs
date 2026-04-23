using ADTO.DCloud.Attendances.Attendance.Dto;
using ADTO.DCloud.Attendances.DingDingAttLogs.Dto;
using ADTO.DCloud.Attendances;
using ADTOSharp.AutoMapper;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.DingDingAttLogs
{
    /// <summary>
    /// 钉钉考勤
    /// </summary>
    public class DingDingAttLogsAppService : DCloudAppServiceBase, IDingDingAttLogsAppService
    {

        #region ctor
        private readonly IRepository<DingdingUserAttLog, Guid> _repository;
        public DingDingAttLogsAppService(IRepository<DingdingUserAttLog, Guid> repository)
        {
            _repository = repository;

        }
        #endregion

        #region 获取员工钉钉打卡记录
        /// <summary>
        /// 获取员工钉钉打卡记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DingdingUserAttLogDto>> GetDingdingUserAttLog(GetDingdingUserAttLogInput input)
        {
            var query = _repository.GetAll()
                .Where(x => x.Timestamp >= input.StartDate && x.Timestamp <= input.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Equals(input.Keyword) || x.UserName.Equals(input.Keyword))
                ;
            var list = await query.ToDynamicListAsync();

            return ObjectMapper.Map<List<DingdingUserAttLogDto>>(list);
        }
        #endregion


    }
}
