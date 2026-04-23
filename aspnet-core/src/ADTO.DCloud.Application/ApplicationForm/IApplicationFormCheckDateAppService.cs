using ADTO.DCloud.ApplicationForm.Dto;
using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm
{
    /// <summary>
    /// 表单公用验证
    /// </summary>
    public interface IApplicationFormCheckDateAppService : IApplicationService
    {
        /// <summary>
        /// 获取指定用户是否禁用指定申请单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resourceTable"></param>
        /// <returns></returns>
        Task<List<DisableUserWrokFlowDto>> GetDisableUserListByUserId(Guid userId, string resourceTable);
        /// <summary>
        /// 判断时间-表单申请时判断申请时间是否在有效期内
        /// </summary>
        /// <param name="date">申请开始时间</param>
        /// <param name="type">表单类型（请假-Abs,出差-Onb,外出-Out,考勤异常-Att）</param>
        /// <param name="isAudit">判断是审核还是申请</param>
        /// <returns>满足条件返回false,不满足返回true</returns>
        Task<bool> IsDateValid(DateTime date, string type, bool isAudit = false);
        /// <summary>
        /// 是否存在时间段内的申请单（请假、外出、出差）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task IsExistApplication(Guid userId, DateTime startime, DateTime endtime, Guid? id);
    }
}
