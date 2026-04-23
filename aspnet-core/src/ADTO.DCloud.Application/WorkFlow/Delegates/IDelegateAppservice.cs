using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.Delegate;
using ADTO.DCloud.WorkFlow.Delegates.Dto;
using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegates
{
    /// <summary>
    /// 流程委托
    /// </summary>
    public interface IDelegateAppservice: IApplicationService
    {
        /// <summary>
        /// 获取关联的模板数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<WorkFlowDelegateRelation>> GetRelationList(Guid id);

        /// <summary>
        /// 根据委托人获取委托记录
        /// </summary>
        /// <param name="type">0 审批委托 1发起委托</param>
        /// <returns></returns>
        public Task<IEnumerable<WorkFlowDelegateruleDto>> GetByToUserIdList(int type=0);
    }
}
