using ADTO.DCloud.DeptRoles.Dto;
using ADTO.DCloud.EmployeeManager.Dto;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public interface IEmployeeInfoAppService
    {
        /// <summary>
        /// 获取分页列表数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<EmployeeInfoDto>> GetAllPageListAsync(GetEmployeePagedInput input);
        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<EmployeeInfoDto> GetAsync(EntityDto<Guid> input);
        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteAsync(EntityDto<Guid> input);
        /// <summary>
        /// 修改用户开户审批状态（默认改为已审批）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateApprovalStatusAsync(List<Guid> Ids);
        /// <summary>
        ///  禁用/启用员工状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateIsActiveAsync(UpdateIsActiveDto input);
    }
}
